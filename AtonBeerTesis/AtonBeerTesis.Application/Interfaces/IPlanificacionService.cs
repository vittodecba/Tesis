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
        Task<string> StockSuficiente(int recetaId);
        Task<IEnumerable<object>> GetInsumosCalculadosAsync(int recetaId);
        Task AsignarFermentadorAsync(int loteId, int fermentadorId);
    }
}