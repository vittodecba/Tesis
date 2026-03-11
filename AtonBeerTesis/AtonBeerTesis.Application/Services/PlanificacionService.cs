using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (dto.FechaProduccion < DateTime.Now)
            {
                throw new Exception("La fecha de producción no puede ser menor a la fecha actual.");
            }

            var ocupado = await _repository.ExisteFermentadorOcupado(dto.FermentadorId, dto.FechaProduccion);
            if (ocupado)
            {
                throw new Exception("El fermentador seleccionado ya está ocupado en la fecha indicada.");
            }

            var mensajeError = await StockSuficiente(dto.RecetaId);
            if (mensajeError != null)
            {
                throw new Exception(mensajeError);
            }

            var planificacion = new PlanificacionProduccion
            {
                RecetaId = dto.RecetaId,
                FermentadorId = dto.FermentadorId,
                FechaProduccion = dto.FechaProduccion,
                Observaciones = dto.Observaciones,
                UsuarioId = dto.UsuarioId,
                Estado = "1",
                FechaCreacion = DateTime.Now
            };

            await _repository.CreateAsync(planificacion);
            return dto;
        }

        public async Task<IEnumerable<PlanificacionProduccionDto>> GetAllAsync()
        {
            var planificaicon = await _repository.GetAllAsync();
            return planificaicon.Select(p => new PlanificacionProduccionDto
            {
                RecetaId = p.RecetaId,
                FermentadorId = p.FermentadorId,
                FechaProduccion = p.FechaProduccion,
                Observaciones = p.Observaciones,
                UsuarioId = p.UsuarioId,
            }).ToList();
        }

        public async Task<string> StockSuficiente(int recetaId)
        {
            var insumosRequeridos = await _repository.GetInsumosByRecetaIdAsync(recetaId);
            if (!insumosRequeridos.Any())
                return "La receta seleccionada no contiene ningun insumo cargado.";

            foreach (var i in insumosRequeridos)
            {
                if (i.Cantidad > i.Insumo.StockActual)
                {
                    return $"Falta de stock de {i.Insumo.NombreInsumo}. Necesitas {i.Cantidad}. Actualmente hay {i.Insumo.StockActual}";
                }
            }
            return null;
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