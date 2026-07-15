using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
using AtonBeerTesis.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Services
{
    public class PlanificacionService : IPlanificacionService
    {
        private readonly IPlanificacionRepository _repository;
        private readonly ILoteRepository _loteRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRepository<ProductoStock> _productoStockRepository;
        private readonly IRepository<MovimientoStock> _movimientoStockRepository;
        private readonly IBarrilRepository _barrilRepository;

        public PlanificacionService(
            IPlanificacionRepository repository,
            ILoteRepository loteRepository,
            IUsuarioRepository usuarioRepository,
            IRepository<ProductoStock> productoStockRepository,
            IRepository<MovimientoStock> movimientoStockRepository,
            IBarrilRepository barrilRepository)
        {
            _repository = repository;
            _loteRepository = loteRepository;
            _usuarioRepository = usuarioRepository;
            _productoStockRepository = productoStockRepository;
            _movimientoStockRepository = movimientoStockRepository;
            _barrilRepository = barrilRepository;
        }

        public async Task<PlanificacionProduccionDto> PLanificarProduccion(PlanificacionProduccionDto dto)
        {
            if (dto.FechaInicio < DateTime.Now.Date)
                throw new Exception("La fecha de producción no puede ser menor a la fecha actual.");

            if (dto.FechaFinEstimada <= dto.FechaInicio)
                throw new Exception("La fecha fin de producción no puede ser menor o igual a la de inicio.");

            var ocupado = await _repository.ExisteFermentadorOcupado(dto.FermentadorId ?? 0, dto.FechaInicio, dto.FechaFinEstimada);
            if (ocupado)
                throw new Exception("El fermentador seleccionado ya está ocupado en la fecha indicada.");

            // Buscar el usuario para setear Responsable
            string responsable = "Sin asignar";
            if (dto.UsuarioId > 0)
            {
                var usuario = await _usuarioRepository.GetByIdAsync(dto.UsuarioId);
                if (usuario != null)
                    responsable = $"{usuario.Nombre} {usuario.Apellido}";
            }

            var nuevoLote = new Lote
            {
                RecetaId = dto.RecetaId,
                VolumenLitros = dto.VolumenLitros,
                CodigoLote = $"L-{DateTime.Now:yyyyMMdd}-{new Random().Next(100, 999)}",
                Estado = EstadoLote.Planificado,
                FechaCreacion = DateTime.Now,
                FermentadorId = dto.FermentadorId,
                FechaElaboracion = dto.FechaInicio,
                Responsable = responsable,
                DiasEstimadosFermentacion = (int)(dto.FechaFinEstimada - dto.FechaInicio).TotalDays,                
            };

            var loteGuardado = await _loteRepository.CreateAsync(nuevoLote);

            var mensajeError = await StockSuficientePorLote(loteGuardado.Id);
            if (mensajeError != null)
            {
                // El stock se valida recién acá porque StockSuficientePorLote necesita el lote
                // persistido (lee los insumos de la receta por LoteId). Si falla, hay que eliminar
                // el lote recién creado: de lo contrario queda un lote "fantasma" en estado
                // Planificado, sin su PlanificacionProduccion, que ensucia el listado y el
                // "lote activo" del fermentador.
                await _loteRepository.DeleteByIdAsync(loteGuardado.Id);
                throw new Exception(mensajeError);
            }

            var planificacion = new PlanificacionProduccion
            {
                LoteId = loteGuardado.Id,
                FermentadorId = dto.FermentadorId,
                FechaInicio = dto.FechaInicio,
                FechaFinEstimada = dto.FechaFinEstimada,
                Observaciones = dto.Observaciones,
                UsuarioId = dto.UsuarioId,
                Estado = EstadoLote.Planificado,
                FechaCreacion = DateTime.Now
            };

            await _repository.CreateAsync(planificacion);

            // NO se marca el fermentador como Ocupado: una planificación futura es una RESERVA,
            // no una ocupación física. El fermentador solo pasa a Ocupado cuando el lote arranca
            // (EnProceso). La doble-reserva se previene por fechas (ExisteFermentadorOcupado).

            dto.LoteId = loteGuardado.Id;
            return dto;
        }

        public async Task<IEnumerable<PlanificacionProduccionDto>> GetAllAsync()
        {
            var planificaciones = await _repository.GetAllAsync();
            var lista = planificaciones.ToList();
            var hoy = DateTime.Now.Date;

            // Fermentadores que ya tienen un lote EnProceso: no pueden recibir otro hasta liberarse.
            // Un fermentador solo admite UN lote EnProceso a la vez.
            var fermentadoresOcupados = lista
                .Where(p => p.Estado == EstadoLote.EnProceso && p.FermentadorId.HasValue)
                .Select(p => p.FermentadorId!.Value)
                .ToHashSet();

            // Auto-transición: Planificado → EnProceso cuando llega la FechaInicio.
            // Se ordena por FechaInicio (y desempata por Id) para que arranque la reserva más antigua.
            var aTransicionar = lista
                .Where(p => p.Estado == EstadoLote.Planificado && p.FechaInicio.Date <= hoy)
                .OrderBy(p => p.FechaInicio)
                .ThenBy(p => p.Id)
                .ToList();

            foreach (var p in aTransicionar)
            {
                // Si el fermentador ya tiene un lote EnProceso (previo o transicionado en este mismo
                // loop), esta reserva espera y queda en Planificado.
                if (p.FermentadorId.HasValue && fermentadoresOcupados.Contains(p.FermentadorId.Value))
                    continue;

                // Descontar stock de insumos (igual que la transición manual)
                var recetaInsumos = await _loteRepository.GetRecetaInsumosByLoteIdAsync(p.LoteId);
                if (p.Lote?.Receta != null && p.Lote.Receta.BatchSizeLitros > 0)
                {
                    foreach (var ri in recetaInsumos)
                    {
                        if (ri.Insumo != null)
                        {
                            decimal cantidadAConsumir = (ri.Cantidad / p.Lote.Receta.BatchSizeLitros) * p.Lote.VolumenLitros;
                            ri.Insumo.StockActual -= cantidadAConsumir;
                            if (ri.Insumo.StockActual < 0) ri.Insumo.StockActual = 0;
                        }
                    }
                }

                p.Estado = EstadoLote.EnProceso;
                if (p.Lote != null)
                {
                    p.Lote.Estado = EstadoLote.EnProceso;
                    // BUG 7: FechaElaboracion = fecha real de inicio del proceso
                    p.Lote.FechaElaboracion = DateTime.Today;
                }

                // Al arrancar la producción, el fermentador pasa a Ocupado (se ocupa físicamente
                // ahora). Se ocupa aunque estuviera Sucio/Mantenimiento: el lote empieza igual.
                if (p.FermentadorId.HasValue)
                {
                    // Bloquea a otras reservas del mismo fermentador en este mismo loop.
                    fermentadoresOcupados.Add(p.FermentadorId.Value);

                    var ferm = await _repository.GetFermentadorByIdAsync(p.FermentadorId.Value);
                    if (ferm != null)
                    {
                        ferm.Estado = EstadoFermentador.Ocupado;
                        await _repository.UpdateFermentadorAsync(ferm);
                    }
                }

                await _repository.UpdateAsync(p);
            }

            return lista.Select(p => new PlanificacionProduccionDto
            {
                Id = p.Id,
                LoteId = p.LoteId,
                FermentadorId = p.FermentadorId,
                FechaInicio = p.FechaInicio,
                FermentadorNombre = p.Fermentador?.Nombre ?? "Sin asignar",
                FechaFinEstimada = p.FechaFinEstimada,
                Observaciones = p.Observaciones,
                UsuarioId = p.UsuarioId,
                RecetaId = p.Lote != null ? p.Lote.RecetaId : 0,
                RecetaNombre = p.Lote?.Receta?.Nombre,
                Estilo = p.Lote?.Receta?.Estilo,
                VolumenLitros = p.Lote != null ? p.Lote.VolumenLitros : 0,
                Estado = p.Estado,
                FechaCreacion = p.FechaCreacion
            }).ToList();
        }

        public async Task<string> StockSuficientePorLote(int LoteId)
        {
            var insumosRequeridos = await _loteRepository.GetRecetaInsumosByLoteIdAsync(LoteId);
            var lote = await _loteRepository.GetByIdAsync(LoteId);

            if (!insumosRequeridos.Any())
                return "La receta seleccionada no contiene insumos cargados.";

            foreach (var i in insumosRequeridos)
            {
                // 1. Calculamos la proporción del lote respecto al batch size de la receta
                decimal proporcionLote = (decimal)lote.VolumenLitros / (decimal)lote.Receta.BatchSizeLitros;
                // 2. Calculamos la cantidad necesaria del insumo para el lote actual, proporcional al volumen
                decimal camtidadUnidadReceta = i.Cantidad * proporcionLote;
                // 3. Aplicamos el factor de conversión de la unidad de medida de la receta (si existe)
                decimal factor = (decimal)(i.unidadMedida?.Factor ?? 1.0);
                // 4. Obtenemos la cantidad necesaria en la unidad base del stock
                decimal cantidadNecesariaBase = camtidadUnidadReceta * factor;
                // 5. Comparamos con el stock actual del insumo, teniendo en cuenta la unidad de medida del stock
                string unidadStock = (i.Insumo.unidadMedida?.Abreviatura ?? "").Trim().ToLower();

                if (cantidadNecesariaBase > i.Insumo.StockActual)// Si la cantidad necesaria en la unidad base es mayor que el stock actual, hay insuficiencia
                {
                    return $"Stock insuficiente para el insumo '{i.Insumo?.NombreInsumo ?? "Sin nombre"}'. " +
                           $"Cantidad necesaria: {Math.Round(cantidadNecesariaBase, 2)} {unidadStock}, " + // <-- Usar unidadStock
                           $"Stock actual: {Math.Round(i.Insumo.StockActual, 2)} {unidadStock}.";
                }
            }

            return null;
        }

        public async Task<PlanificacionProduccionDto> ActualizarPlanificacion(int loteId, PlanificacionProduccionDto dto)
        {
            var planificacion = await _repository.GetByLoteIdAsync(loteId);
            if (planificacion == null)
                throw new Exception($"No se encontró la planificación con Lote ID {loteId}.");

            // Guard: una planificación de un lote ya cerrado no admite cambios.
            if (planificacion.Estado == EstadoLote.Finalizado || planificacion.Estado == EstadoLote.Descartado)
                throw new Exception($"El lote ya está {planificacion.Estado} y no admite cambios.");

            if (planificacion.Estado != EstadoLote.Planificado)
            {
                if (planificacion.FermentadorId != dto.FermentadorId)
                    throw new Exception("No se puede cambiar el fermentador una vez iniciada la producción");
                if (planificacion.Lote.RecetaId != dto.RecetaId)
                    throw new Exception("No se puede cambiar la receta una vez iniciada la producción");
            }

            // Guardamos el valor inicial antes de modificarlo
            var volumenAnterior = planificacion.Lote.VolumenLitros;
            var recetaAnterior = planificacion.Lote.RecetaId;
            // Seteamos los nuevos valores temporalmente para las validaciones
            planificacion.Lote.VolumenLitros = dto.VolumenLitros;
            planificacion.Lote.RecetaId = dto.RecetaId;

            //Validar el stock al editar el volumen ingresado
            if (planificacion.Estado == EstadoLote.Planificado)
            {
                var mensajeError = await StockSuficientePorLote(loteId);
                if (mensajeError != null)
                {
                    // Revertimos los cambios en el objeto para que no se guarden
                    planificacion.Lote.VolumenLitros = volumenAnterior;
                    planificacion.Lote.RecetaId = recetaAnterior;
                    throw new Exception("Error de stock: " + mensajeError);
                }
            }
            // Solo validamos y descontamos stock si pasamos de Planificado a En Proceso
            if (planificacion.Estado == EstadoLote.Planificado && dto.Estado == EstadoLote.EnProceso)
            {
                // 0. Un fermentador solo puede tener UN lote EnProceso a la vez: si ya hay otro
                // lote en proceso en ese fermentador, no se puede arrancar este hasta finalizarlo.
                if (dto.FermentadorId.HasValue)
                {
                    var loteActivo = await _loteRepository.GetActivoByFermentadorIdAsync(dto.FermentadorId.Value);
                    if (loteActivo != null && loteActivo.Id != planificacion.LoteId)
                        throw new Exception("Ese fermentador ya tiene un lote en proceso. Debe finalizarlo antes de iniciar otro.");

                    // 0.b Respetar el ORDEN de planificación: no se puede arrancar este lote si hay
                    // una reserva anterior (fecha más temprana) todavía pendiente en el fermentador.
                    var reservaAnterior = await _repository.GetReservaAnteriorPendienteAsync(
                        dto.FermentadorId.Value, dto.FechaInicio, planificacion.Id);
                    if (reservaAnterior != null)
                        throw new Exception(
                            $"Debe respetar el orden de planificación: primero finalice, descarte o elimine el lote " +
                            $"'{reservaAnterior.Lote?.Codigo}' (planificado para el {reservaAnterior.FechaInicio:dd/MM/yyyy}) " +
                            $"antes de iniciar este.");
                }

                // 1. Validar Stock
                var mensajeError = await StockSuficientePorLote(loteId);
                if (mensajeError != null)
                {
                    planificacion.Lote.VolumenLitros = volumenAnterior; // Revertimos si falla
                    throw new Exception("Error de stock: " + mensajeError);
                }

                // 2. Descontar Stock
                var recetaInsumos = await _loteRepository.GetRecetaInsumosByLoteIdAsync(loteId);
                foreach (var ri in recetaInsumos)
                {
                    if (ri.Insumo != null)
                    {
                        decimal factor = (decimal)(ri.unidadMedida?.Factor ?? 1.0);
                        decimal cantidadSegunReceta = (ri.Cantidad / (decimal)planificacion.Lote.Receta.BatchSizeLitros) * (decimal)planificacion.Lote.VolumenLitros;
                        decimal cantidadAConsumirEnBase = cantidadSegunReceta * factor;

                        ri.Insumo.StockActual -= cantidadAConsumirEnBase;
                        if (ri.Insumo.StockActual < 0) ri.Insumo.StockActual = 0;
                    }
                }
            }            
            if (dto.FechaInicio != planificacion.FechaInicio && dto.FechaInicio < DateTime.Now.Date)
                throw new Exception("La fecha de inicio no puede ser menor a la fecha actual.");

            if (dto.FechaFinEstimada <= dto.FechaInicio)
                throw new Exception("La fecha fin no puede ser menor o igual a la de inicio.");

            // Transición a Finalizado o Descartado
            bool esTransicionFinal = (dto.Estado == EstadoLote.Finalizado || dto.Estado == EstadoLote.Descartado)
                && planificacion.Estado != EstadoLote.Finalizado
                && planificacion.Estado != EstadoLote.Descartado;

            if (esTransicionFinal)
            {
                var fermentadorAFinalizar = planificacion.FermentadorId.HasValue
                    ? await _repository.GetFermentadorByIdAsync(planificacion.FermentadorId.Value)
                    : null;
                if (fermentadorAFinalizar != null)
                {
                    fermentadorAFinalizar.Estado = EstadoFermentador.Sucio;
                    await _repository.UpdateFermentadorAsync(fermentadorAFinalizar);
                }
                if (planificacion.Lote != null)
                    planificacion.Lote.FechaFinReal = DateTime.Now;

                if (dto.Estado == EstadoLote.Finalizado)
                {
                    var loteConDesignaciones = await _loteRepository.GetByIdAsync(planificacion.LoteId);

                    var volumenDesignado = loteConDesignaciones?.Designaciones?
                        .Sum(d => d.VolumenAsignado) ?? 0;
                    if (volumenDesignado < (decimal)(loteConDesignaciones?.VolumenLitros ?? 0))
                        throw new InvalidOperationException(
                            $"No se puede finalizar: {volumenDesignado}L designados de " +
                            $"{loteConDesignaciones?.VolumenLitros}L totales. " +
                            "Completá la designación de volumen en el detalle del lote.");

                    if (loteConDesignaciones?.Designaciones?.Any() == true)
                    {
                        var estiloLote = loteConDesignaciones.Estilo ?? loteConDesignaciones.Receta?.Estilo ?? string.Empty;
                        var todosLosProductos = await _productoStockRepository.FindAllAsync();

                        // PRE-VALIDACIÓN DE BARRILES
                        var formatosRetornables = await _barrilRepository.ObtenerFormatosRetornablesAsync();
                        var barrilesReservados = new Dictionary<int, List<Domain.Entities.Barril>>();
                        foreach (var des in loteConDesignaciones.Designaciones)
                        {
                            if (!formatosRetornables.TryGetValue(des.FormatoEnvaseId, out var capacidad)) continue;
                            int unidadesReq = (int)(des.VolumenAsignado / capacidad);
                            var disponibles = await _barrilRepository.GetDisponiblesAsync(des.FormatoEnvaseId, unidadesReq);
                            if (disponibles.Count < unidadesReq)
                                throw new InvalidOperationException(
                                    $"No hay barriles suficientes para el formato {des.FormatoEnvaseId}: " +
                                    $"se necesitan {unidadesReq} y solo hay {disponibles.Count} disponible(s).");
                            barrilesReservados[des.FormatoEnvaseId] = disponibles;
                        }

                        foreach (var designacion in loteConDesignaciones.Designaciones)
                        {
                            var formato = designacion.FormatoEnvase;
                            if (formato == null || formato.CapacidadLitros <= 0) continue;

                            var unidades = designacion.VolumenAsignado / formato.CapacidadLitros;

                            var productoStock = todosLosProductos.FirstOrDefault(p =>
                                p.FormatoEnvaseId == designacion.FormatoEnvaseId &&
                                p.Estilo.Equals(estiloLote, StringComparison.OrdinalIgnoreCase) &&
                                p.RecetaId == loteConDesignaciones.RecetaId);

                            if (productoStock == null && !string.IsNullOrWhiteSpace(estiloLote))
                            {
                                productoStock = new ProductoStock
                                {
                                    FormatoEnvaseId = designacion.FormatoEnvaseId,
                                    Estilo = estiloLote,
                                    RecetaId = loteConDesignaciones.RecetaId,
                                    StockActual = 0
                                };
                                await _productoStockRepository.AddAsync(productoStock);
                                todosLosProductos.Add(productoStock);
                            }
                            if (productoStock == null) continue;

                            var stockPrevio = productoStock.StockActual;
                            productoStock.StockActual += unidades;
                            await _movimientoStockRepository.AddAsync(new MovimientoStock
                            {
                                ProductoStockId = productoStock.Id,
                                LoteId = loteConDesignaciones.Id,
                                Cantidad = unidades,
                                TipoMovimiento = "Ingreso",
                                MotivoMovimiento = "Produccion",
                                StockPrevio = stockPrevio,
                                StockResultante = productoStock.StockActual,
                                Fecha = DateTime.Now
                            });
                        }

                        // LLENAR BARRILES con SQL directo
                        var idsALlenar = barrilesReservados.Values
                            .SelectMany(lista => lista.Select(b => b.Id))
                            .ToList();
                        if (idsALlenar.Any())
                            await _barrilRepository.MarcarComoLlenosAsync(idsALlenar, planificacion.LoteId);
                    }
                }
            }

            bool cambioFermentadorOFechas = planificacion.FermentadorId != dto.FermentadorId ||
                                             planificacion.FechaInicio != dto.FechaInicio ||
                                             planificacion.FechaFinEstimada != dto.FechaFinEstimada;

            if (cambioFermentadorOFechas)
            {
                var ocupado = await _repository.ExisteFermentadorOcupado(dto.FermentadorId ?? 0, dto.FechaInicio, dto.FechaFinEstimada, loteId);
                if (ocupado)
                    throw new Exception("El fermentador ya está ocupado en ese rango de fechas.");
            }

            // El fermentador solo pasa a Ocupado cuando el lote está EnProceso (ocupación física).
            // Cambiar el fermentador de una reserva Planificado NO ocupa el nuevo ni libera el anterior
            // por flag (una reserva no ocupa el flag; la doble-reserva se previene por fechas más arriba).
            if (dto.Estado == EstadoLote.EnProceso && dto.FermentadorId.HasValue)
            {
                var fermentadorActual = await _repository.GetFermentadorByIdAsync(dto.FermentadorId.Value);
                if (fermentadorActual != null && fermentadorActual.Estado != EstadoFermentador.Ocupado)
                {
                    fermentadorActual.Estado = EstadoFermentador.Ocupado;
                    await _repository.UpdateFermentadorAsync(fermentadorActual);
                }
            }

            planificacion.FermentadorId = dto.FermentadorId;
            planificacion.FechaInicio = dto.FechaInicio;
            planificacion.FechaFinEstimada = dto.FechaFinEstimada;
            planificacion.Observaciones = dto.Observaciones;
            planificacion.Estado = dto.Estado;

            if (planificacion.Lote != null)
            {
                planificacion.Lote.RecetaId = dto.RecetaId;
                planificacion.Lote.VolumenLitros = dto.VolumenLitros;
                planificacion.Lote.FermentadorId = dto.FermentadorId;
                planificacion.Lote.DiasEstimadosFermentacion = (int)(dto.FechaFinEstimada - dto.FechaInicio).TotalDays;
                planificacion.Lote.Estado = dto.Estado;
                planificacion.Lote.Observaciones = dto.Observaciones; // FIX: sincronizar con lote-detalle y fermentador-detalle
            }

            await _repository.UpdateAsync(planificacion);

            dto.LoteId = loteId;
            return dto;
        }


        public async Task AsignarFermentadorAsync(int loteId, int fermentadorId)
        {
            var lotes = await _repository.GetAllAsync();
            var lote = lotes.FirstOrDefault(x => x.Id == loteId);
            if (lote == null) throw new Exception("Lote no encontrado.");

            // BUG 10: Solo se puede cambiar fermentador si el lote está Planificado (no EnProceso)
            if (lote.Estado == EstadoLote.EnProceso)
                throw new Exception("No se puede cambiar el fermentador de un lote que ya está En Proceso.");

            var ocupado = await _repository.ExisteFermentadorOcupado(fermentadorId, lote.FechaInicio, lote.FechaFinEstimada, loteId);
            if (ocupado)
                throw new Exception("El fermentador ya está ocupado en ese rango de fechas.");

            // Solo opera sobre lotes Planificado (reservas, guard arriba). Una reserva NO ocupa el
            // flag del fermentador: no se libera el anterior ni se marca Ocupado el nuevo. El flag
            // Ocupado solo se setea cuando el lote arranca (EnProceso).
            var nuevo = await _repository.GetFermentadorByIdAsync(fermentadorId);
            if (nuevo == null) throw new Exception("Fermentador no encontrado.");

            // Actualizar PlanificacionProduccion.FermentadorId
            lote.FermentadorId = fermentadorId;

            // BUG 9/10: También actualizar Lote.FermentadorId para que GetActivoByFermentadorIdAsync funcione
            if (lote.Lote != null)
            {
                lote.Lote.FermentadorId = fermentadorId;
                await _loteRepository.UpdateAsync(lote.Lote);
            }

            await _repository.UpdateAsync(lote);
        }

        public async Task<IEnumerable<object>> GetInsumosCalculadosAsync(int planificacionId)
        {
            var planificacion = await _repository.GetByIdAsync(planificacionId);

            if (planificacion == null || planificacion.Lote == null) return Enumerable.Empty<object>();

            var lote = planificacion.Lote;
            var receta = lote.Receta;

            if (receta == null) return Enumerable.Empty<object>();
            var insumos = await _loteRepository.GetRecetaInsumosByLoteIdAsync(lote.Id);
            return insumos.Select(i => new {
                Material = i.Insumo?.NombreInsumo ?? "Sin nombre",
                CantidadTotal = Math.Round((i.Cantidad / receta.BatchSizeLitros) * lote.VolumenLitros, 2),
                Unidad = i.unidadMedida?.Nombre ?? "Unid"
            });
        }

        public async Task<bool> EliminarPlanificacionAsync(int id)
        {
            var planificacion = await _repository.GetByLoteIdAsync(id);
            if (planificacion == null) return false;

            using var transaction = await _repository.BeginTransactionAsync();
            try
            {
                var fermentador = planificacion.FermentadorId.HasValue
                    ? await _repository.GetFermentadorByIdAsync(planificacion.FermentadorId.Value)
                    : null;
                if (fermentador != null && planificacion.Estado == EstadoLote.EnProceso)
                {
                    // Solo un lote EnProceso ocupa físicamente el fermentador → al borrarlo, queda Sucio.
                    // Si era Planificado (reserva), NUNCA ocupó el flag: no se toca el fermentador
                    // (podría estar Ocupado/Sucio por otro lote que comparte el mismo tanque).
                    // Finalizado/Descartado: el fermentador ya quedó Sucio durante la finalización, no tocar.
                    fermentador.Estado = EstadoFermentador.Sucio;
                    await _repository.UpdateFermentadorAsync(fermentador);
                }

                // Eliminar el Lote — EF Core cascade borra PlanificacionProduccion, SQL cascade borra RegistrosFermentacion
                await _loteRepository.DeleteByIdAsync(planificacion.LoteId);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}