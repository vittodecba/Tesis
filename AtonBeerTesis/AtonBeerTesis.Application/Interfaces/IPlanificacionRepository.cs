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
        Task<PlanificacionProduccion> UpdateAsync(PlanificacionProduccion planificacion);
        Task<bool> ExisteFermentadorOcupado(int fermentadorId, DateTime fecha);
        Task<IEnumerable<RecetaInsumo>> GetInsumosByRecetaIdAsync(int recetaId);

        Task<Fermentador> GetFermentadorByIdAsync(int id);
        Task UpdateFermentadorAsync(Fermentador fermentador);
    }
}