using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface ILoteRepository
    {
        Task<LotePrueba> AddAsync(LotePrueba lote);
        Task<LotePrueba?> GetByIdAsync(int id);
        Task<List<LotePrueba>> GetAllAsync();
        Task<LotePrueba?> GetActivoByFermentadorIdAsync(int fermentadorId);
        Task<bool> ExisteCodigoAsync(string codigo);
        Task<bool> UpdateAsync(LotePrueba lote);
    }
}