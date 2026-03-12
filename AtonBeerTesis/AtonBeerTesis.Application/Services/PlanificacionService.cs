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

        public PlanificacionService(IPlanificacionRepository repository)
        {
            _repository = repository;
        }

        public async Task<PlanificacionProduccionDto> PLanificarProduccion(PlanificacionProduccionDto dto)
        {
            var planificacion = new PlanificacionProduccion
            {
                RecetaId = dto.RecetaId,
                FermentadorId = dto.FermentadorId,
                FechaProduccion = dto.FechaProduccion,
                UsuarioId = dto.UsuarioId,
                Estado = "1",
                FechaCreacion = DateTime.Now
            };

            await _repository.CreateAsync(planificacion);
            return dto;
        }

        public async Task<IEnumerable<PlanificacionProduccionDto>> GetAllAsync()
        {
            var planificacion = await _repository.GetAllAsync();
            return planificacion.Select(p => new PlanificacionProduccionDto
            {
                Id = p.Id,
                RecetaId = p.RecetaId,
                FermentadorId = p.FermentadorId,
                FermentadorNombre = p.fermentador?.Nombre ?? "Sin asignar",
                FechaProduccion = p.FechaProduccion,
                Estado = p.Estado,
                Observaciones = p.Observaciones, 
                UsuarioId = p.UsuarioId        
            }).ToList();
        }

        public async Task<string?> StockSuficiente(int recetaId)
        {
            var insumosRequeridos = await _repository.GetInsumosByRecetaIdAsync(recetaId);
            foreach (var i in insumosRequeridos)
            {
                if (i.Cantidad > i.Insumo.StockActual)
                    return $"Falta de stock de {i.Insumo.NombreInsumo}.";
            }
            return null;
        }

        public async Task AsignarFermentadorAsync(int loteId, int fermentadorId)
        {
            var lotes = await _repository.GetAllAsync();
            var lote = lotes.FirstOrDefault(x => x.Id == loteId);
            if (lote == null) throw new Exception("Lote no encontrado.");

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

        public async Task<IEnumerable<object>> GetInsumosCalculadosAsync(int recetaId)
        {
            var insumos = await _repository.GetInsumosByRecetaIdAsync(recetaId);
            return insumos.Select(i => new {
                Material = i.Insumo.NombreInsumo,
                CantidadTotal = i.Cantidad,
                Unidad = "unid"
            });
        }
    }
}