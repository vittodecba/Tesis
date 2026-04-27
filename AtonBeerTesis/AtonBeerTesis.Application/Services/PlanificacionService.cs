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

        public PlanificacionService(
            IPlanificacionRepository repository,
            ILoteRepository loteRepository,
            IUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _loteRepository = loteRepository;
            _usuarioRepository = usuarioRepository;
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
                throw new Exception(mensajeError);

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

            // Marcar fermentador como Ocupado
            var fermentador = await _repository.GetFermentadorByIdAsync(dto.FermentadorId ?? 0);
            if (fermentador != null)
            {
                fermentador.Estado = EstadoFermentador.Ocupado;
                await _repository.UpdateFermentadorAsync(fermentador);
            }

            dto.LoteId = loteGuardado.Id;
            return dto;
        }

        public async Task<IEnumerable<PlanificacionProduccionDto>> GetAllAsync()
        {
            var planificaciones = await _repository.GetAllAsync();
            var lista = planificaciones.ToList();
            var hoy = DateTime.Now.Date;

            // Auto-transición: Planificado → EnProceso cuando llega la FechaInicio
            var aTransicionar = lista
                .Where(p => p.Estado == EstadoLote.Planificado && p.FechaInicio.Date <= hoy)
                .ToList();

            foreach (var p in aTransicionar)
            {
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
                    p.Lote.Estado = EstadoLote.EnProceso;

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

            if (planificacion.FermentadorId != dto.FermentadorId)
            {
                var fermentadorAnterior = planificacion.FermentadorId.HasValue
                    ? await _repository.GetFermentadorByIdAsync(planificacion.FermentadorId.Value)
                    : null;
                if (fermentadorAnterior != null)
                {
                    fermentadorAnterior.Estado = EstadoFermentador.Disponible;
                    await _repository.UpdateFermentadorAsync(fermentadorAnterior);
                }

                var fermentadorNuevo = await _repository.GetFermentadorByIdAsync(dto.FermentadorId ?? 0);
                if (fermentadorNuevo != null)
                {
                    fermentadorNuevo.Estado = EstadoFermentador.Ocupado;
                    await _repository.UpdateFermentadorAsync(fermentadorNuevo);
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

            var ocupado = await _repository.ExisteFermentadorOcupado(fermentadorId, lote.FechaInicio, lote.FechaFinEstimada, loteId);
            if (ocupado)
                throw new Exception("El fermentador ya está ocupado en ese rango de fechas.");

            if (lote.FermentadorId.HasValue && lote.FermentadorId.Value != fermentadorId)
            {
                var anterior = await _repository.GetFermentadorByIdAsync(lote.FermentadorId.Value);
                if (anterior != null)
                {
                    anterior.Estado = EstadoFermentador.Disponible;
                    await _repository.UpdateFermentadorAsync(anterior);
                }
            }

            lote.FermentadorId = fermentadorId;
            var nuevo = await _repository.GetFermentadorByIdAsync(fermentadorId);
            if (nuevo != null)
            {
                nuevo.Estado = EstadoFermentador.Ocupado;
                await _repository.UpdateFermentadorAsync(nuevo);
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

            var fermentador = planificacion.FermentadorId.HasValue
                ? await _repository.GetFermentadorByIdAsync(planificacion.FermentadorId.Value)
                : null;
            if (fermentador != null)
            {
                if (planificacion.Estado == EstadoLote.Planificado)
                {
                    // Nunca se usó → liberar fermentador
                    fermentador.Estado = EstadoFermentador.Disponible;
                    await _repository.UpdateFermentadorAsync(fermentador);
                }
                else if (planificacion.Estado == EstadoLote.EnProceso)
                {
                    // Estaba en uso → marcar como Sucio
                    fermentador.Estado = EstadoFermentador.Sucio;
                    await _repository.UpdateFermentadorAsync(fermentador);
                }
                // Finalizado/Descartado: fermentador ya quedó Sucio durante la finalización, no tocar
            }

            // Eliminar el Lote — EF Core cascade borra PlanificacionProduccion, SQL cascade borra RegistrosFermentacion
            await _loteRepository.DeleteByIdAsync(planificacion.LoteId);
            return true;
        }
    }
}