using AtonBeerTesis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IPlanificacionRepository
    {
        Task<PlanificacionProduccion> CreateAsync(PlanificacionProduccion planificacion);
        Task<IEnumerable<PlanificacionProduccion>> GetAllAsync();
        Task<bool> ExisteFermentadorOcupado(int fermentadorId, DateTime fechaInicio, DateTime fechaFin, int excluirLoteId = 0);
        Task<PlanificacionProduccion> GetByIdAsync(int id);
        Task<PlanificacionProduccion> GetByLoteIdAsync(int loteId); // ← nuevo
        Task<PlanificacionProduccion> UpdateAsync(PlanificacionProduccion planificacion);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<RecetaInsumo>> GetInsumosByRecetaIdAsync(int recetaId);
        Task<Fermentador> GetFermentadorByIdAsync(int id);
        Task UpdateFermentadorAsync(Fermentador fermentador);
    }
}