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

            var ocupado = await _repository.ExisteFermentadorOcupado(dto.FermentadorId, dto.FechaInicio, dto.FechaFinEstimada);
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
            var fermentador = await _repository.GetFermentadorByIdAsync(dto.FermentadorId);
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
            var planificacion = await _repository.GetAllAsync();
            return planificacion.Select(p => new PlanificacionProduccionDto
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
                decimal cantidadNecesaria = (i.Cantidad / lote.Receta.BatchSizeLitros) * lote.VolumenLitros;
                if (cantidadNecesaria > i.Insumo.StockActual)
                    return $"Falta de stock de {i.Insumo.NombreInsumo}. Necesitas {cantidadNecesaria:F2}. Actualmente hay {i.Insumo.StockActual:F2}";
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
              if(planificacion.FermentadorId != dto.FermentadorId)
                    throw new Exception("No se puede cambiar el fermentador una vez iniciada la producción");
              if(planificacion.Lote.RecetaId != dto.RecetaId)
                    throw new Exception("No se puede cambiar la receta una vez iniciada la producción");
            }

            if (dto.FechaInicio != planificacion.FechaInicio && dto.FechaInicio < DateTime.Now.Date)
                throw new Exception("La fecha de inicio no puede ser menor a la fecha actual.");

            if (dto.FechaFinEstimada <= dto.FechaInicio)
                throw new Exception("La fecha fin no puede ser menor o igual a la de inicio.");
          
            if (planificacion.Estado == EstadoLote.Planificado && dto.Estado == EstadoLote.EnProceso)
            {                
                var recetaInsumos = await _loteRepository.GetRecetaInsumosByLoteIdAsync(loteId);

                foreach (var ri in recetaInsumos)
                {
                    if (ri.Insumo != null)
                    {
                        // Calculamos la cantidad a consumir proporcional al volumen del lote
                        decimal cantidadAConsumir = (ri.Cantidad / planificacion.Lote.Receta.BatchSizeLitros) * planificacion.Lote.VolumenLitros;
                        // Restamos esa cantidad al stock actual del insumo                      
                        ri.Insumo.StockActual -= cantidadAConsumir;
                        if (ri.Insumo.StockActual < 0) ri.Insumo.StockActual = 0;

                        // Si tenés un InsumoRepository, podrías llamar al Update aquí, 
                        // pero si usás el mismo Contexto, se guarda todo junto al final.
                    }
                }
            }

            bool cambioFermentadorOFechas = planificacion.FermentadorId != dto.FermentadorId ||
                                             planificacion.FechaInicio != dto.FechaInicio ||
                                             planificacion.FechaFinEstimada != dto.FechaFinEstimada;

            if (cambioFermentadorOFechas)
            {
                var ocupado = await _repository.ExisteFermentadorOcupado(dto.FermentadorId, dto.FechaInicio, dto.FechaFinEstimada, loteId);
                if (ocupado)
                    throw new Exception("El fermentador ya está ocupado en ese rango de fechas.");
            }

            // Si cambió el fermentador, actualizar estados de fermentadores
            if (planificacion.FermentadorId != dto.FermentadorId)
            {
                var fermentadorAnterior = await _repository.GetFermentadorByIdAsync(planificacion.FermentadorId);
                if (fermentadorAnterior != null)
                {
                    fermentadorAnterior.Estado = EstadoFermentador.Disponible;
                    await _repository.UpdateFermentadorAsync(fermentadorAnterior);
                }

                var fermentadorNuevo = await _repository.GetFermentadorByIdAsync(dto.FermentadorId);
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

            if (lote.FermentadorId != 0 && lote.FermentadorId != fermentadorId)
            {
                var anterior = await _repository.GetFermentadorByIdAsync(lote.FermentadorId);
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
                Unidad = i.Insumo?.unidadMedida?.Nombre ?? "Unid"
            });
        }

        public async Task<bool> EliminarPlanificacionAsync(int id)
        {
            var planificacion = await _repository.GetByLoteIdAsync(id);
            if (planificacion == null) return false;

            var fermentador = await _repository.GetFermentadorByIdAsync(planificacion.FermentadorId);
            if (fermentador != null)
            {
                // Si estaba EnProceso → Sucio (el tanque estuvo en uso)
                // Si estaba Planificado → Disponible (nunca se usó)
                fermentador.Estado = planificacion.Estado == EstadoLote.EnProceso
                    ? EstadoFermentador.Sucio
                    : EstadoFermentador.Disponible;

                await _repository.UpdateFermentadorAsync(fermentador);
            }

            return await _repository.DeleteAsync(planificacion.Id);
        }
    }
}