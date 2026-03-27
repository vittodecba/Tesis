using AtonBeerTesis.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IPlanificacionService
    {
        Task<PlanificacionProduccionDto> PLanificarProduccion(PlanificacionProduccionDto dto);
        Task<IEnumerable<PlanificacionProduccionDto>> GetAllAsync();
        Task<string> StockSuficientePorLote(int recetaId);
        Task<PlanificacionProduccionDto> ActualizarPlanificacion(int loteId, PlanificacionProduccionDto dto);
        Task<IEnumerable<object>> GetInsumosCalculadosAsync(int planificacionId); // Cambiado: ahora pide el ID de planificación
        Task AsignarFermentadorAsync(int loteId, int fermentadorId);
        Task<bool> EliminarPlanificacionAsync(int id);
    }
}