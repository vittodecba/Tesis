using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IFermentadorRepository
    {
        Task<List<Fermentador>> GetAllAsync();
        Task<List<Fermentador>> GetAllConPlanificacionAsync();
        Task<bool> DeleteAsync(int id);
        Task<Fermentador> AddAsync(Fermentador fermentador);
        Task<Fermentador?> GetByIdAsync(int id);

        // CORRECCIÓN: Ahora recibe la entidad, no el DTO.
        Task<bool> UpdateAsync(Fermentador fermentador);
        Task<bool> TieneLotesAsociadosAsync(int id);
    }
}