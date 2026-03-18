using AtonBeerTesis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IPlanificacionRepository
    {

        Task<PlanificacionProduccion> CreateAsync(PlanificacionProduccion planificacion);//Para crear una nueva planificación de producción
        Task<IEnumerable<PlanificacionProduccion>> GetAllAsync();//Para obtener todas las planificaciones
        Task<bool> ExisteFermentadorOcupado(int fermentadorId, DateTime fechaInicio, DateTime fechaFin, int excluirLoteId=0);//Para verificar si un fermentador está ocupado en una fecha específica
        Task<PlanificacionProduccion> GetByIdAsync(int id);//Para obtener una planificación por su ID
        Task<PlanificacionProduccion> UpdateAsync(PlanificacionProduccion planificacion);//Para actualizar una planificación existente
        Task<bool> DeleteAsync(int id);//Para eliminar una planificación por su ID
        Task<IEnumerable<RecetaInsumo>> GetInsumosByRecetaIdAsync(int recetaId);
        Task<Fermentador> GetFermentadorByIdAsync(int id);
        Task UpdateFermentadorAsync(Fermentador fermentador);
    }
}