using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
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

        public PlanificacionService(IPlanificacionRepository repository, ILoteRepository loteRepository)
        {
            _repository = repository;
            _loteRepository = loteRepository;
        }

        public async Task<PlanificacionProduccionDto> PLanificarProduccion(PlanificacionProduccionDto dto)
        {
            if (dto.FechaInicio < DateTime.Now.Date)
                throw new Exception("La fecha de producción no puede ser menor a la fecha actual.");

            if (dto.FechaFinEstimada <= dto.FechaInicio)
                throw new Exception("La fecha fin de producción no puede ser menor o igual a la de inicio");

            var fermentador = await _repository.GetFermentadorByIdAsync(dto.FermentadorId);
            if (fermentador == null) throw new Exception("El fermentador no existe.");

            if (dto.VolumenLitros > fermentador.Capacidad)
                throw new Exception($"Capacidad insuficiente: El fermentador '{fermentador.Nombre}' solo soporta {fermentador.Capacidad}L y querés ingresar {dto.VolumenLitros}L.");

            var ocupado = await _repository.ExisteFermentadorOcupado(dto.FermentadorId, dto.FechaInicio, dto.FechaFinEstimada);
            if (ocupado) throw new Exception("El fermentador seleccionado ya está ocupado en la fecha indicada.");

            var nuevoLote = new Lote
            {
                RecetaId = dto.RecetaId,
                VolumenLitros = dto.VolumenLitros,
                CodigoLote = $"L-{DateTime.Now:yyyyMMdd}-{new Random().Next(100, 999)}",
                Estado = EstadoLote.Planificado,
                FechaCreacion = DateTime.Now,
                FermentadorId = dto.FermentadorId
            };

            var loteGuardado = await _loteRepository.CreateAsync(nuevoLote);
            var mensajeError = await StockSuficientePorLote(loteGuardado.Id);
            if (mensajeError != null) throw new Exception(mensajeError);

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
                VolumenLitros = p.Lote != null ? p.Lote.VolumenLitros : 0,
                Estado = p.Estado,
                FechaCreacion = p.FechaCreacion
            }).ToList();
        }

        public async Task<string> StockSuficientePorLote(int LoteId)
        {
            var insumosRequeridos = await _loteRepository.GetRecetaInsumosByLoteIdAsync(LoteId);
            var lote = await _loteRepository.GetByIdAsync(LoteId);
            if (!insumosRequeridos.Any()) return "La receta seleccionada no contiene insumos cargados.";

            foreach (var i in insumosRequeridos)
            {
                decimal cantidadNecesaria = (i.Cantidad / lote.Receta.BatchSizeLitros) * lote.VolumenLitros;
                if (cantidadNecesaria > i.Insumo.StockActual)
                {
                    return $"Falta de stock de {i.Insumo.NombreInsumo}. Necesitas {cantidadNecesaria:F2}. Actualmente hay {i.Insumo.StockActual:F2}";
                }
            }
            return null;
        }

        public async Task<PlanificacionProduccionDto> ActualizarPlanificacion(int loteId, PlanificacionProduccionDto dto)
        {
            var planificacion = await _repository.GetByIdAsync(loteId);
            if (planificacion == null) throw new Exception($"No se encontró la planificación con Lote ID {loteId}.");

            if (dto.FechaInicio != planificacion.FechaInicio && dto.FechaInicio < DateTime.Now.Date)
                throw new Exception("La fecha de inicio no puede ser menor a la fecha actual.");

            if (dto.FechaFinEstimada <= dto.FechaInicio)
                throw new Exception("La fecha fin no puede ser menor o igual a la de inicio.");

            bool cambioFermentadorOFechas = planificacion.FermentadorId != dto.FermentadorId ||
                                             planificacion.FechaInicio != dto.FechaInicio ||
                                             planificacion.FechaFinEstimada != dto.FechaFinEstimada;

            if (cambioFermentadorOFechas)
            {
                var ocupado = await _repository.ExisteFermentadorOcupado(dto.FermentadorId, dto.FechaInicio, dto.FechaFinEstimada, loteId);
                if (ocupado) throw new Exception("El fermentador ya está ocupado en ese rango de fechas.");
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
            }

            await _repository.UpdateAsync(planificacion);
            dto.LoteId = loteId;
            return dto;
        }

        public async Task AsignarFermentadorAsync(int loteId, int fermentadorId)
        {
            var planif = await _repository.GetByIdAsync(loteId);
            if (planif == null) throw new Exception("Planificación no encontrada.");

            var ocupado = await _repository.ExisteFermentadorOcupado(fermentadorId, planif.FechaInicio, planif.FechaFinEstimada, loteId);
            if (ocupado) throw new Exception("El fermentador ya está ocupado en ese rango de fechas.");

            // Lógica de Vitto para liberar el anterior
            if (planif.FermentadorId != 0 && planif.FermentadorId != fermentadorId)
            {
                var anterior = await _repository.GetFermentadorByIdAsync(planif.FermentadorId);
                if (anterior != null)
                {
                    anterior.Estado = EstadoFermentador.Disponible;
                    await _repository.UpdateFermentadorAsync(anterior);
                }
            }

            planif.FermentadorId = fermentadorId;
            var nuevo = await _repository.GetFermentadorByIdAsync(fermentadorId);
            if (nuevo != null)
            {
                nuevo.Estado = EstadoFermentador.Ocupado;
                await _repository.UpdateFermentadorAsync(nuevo);
            }

            await _repository.UpdateAsync(planif);
        }

        public async Task<IEnumerable<object>> GetInsumosCalculadosAsync(int planificacionId)
        {
            var planif = await _repository.GetByIdAsync(planificacionId);
            if (planif == null) return Enumerable.Empty<object>();

            var lote = await _loteRepository.GetByIdAsync(planif.LoteId);
            if (lote == null || lote.Receta == null) return Enumerable.Empty<object>();

            var insumosReceta = await _repository.GetInsumosByRecetaIdAsync(lote.RecetaId);

            return insumosReceta.Select(i => new {
                Material = i.Insumo?.NombreInsumo ?? "Sin nombre",
                // Cálculo: (Cantidad de la receta / Litros de la receta) * Litros reales del lote
                CantidadTotal = Math.Round((i.Cantidad / lote.Receta.BatchSizeLitros) * lote.VolumenLitros, 2),
                Unidad = i.Insumo?.unidadMedida?.Nombre ?? "N/A"
            });
        }

        public async Task<bool> EliminarPlanificacionAsync(int id)
        {
            var planif = await _repository.GetByIdAsync(id);
            if (planif == null) return false;

            int loteAsociadoId = planif.LoteId;
            await _repository.DeleteAsync(id);
            await _loteRepository.DeleteByIdAsync(loteAsociadoId);
            return true;
        }
    }
}