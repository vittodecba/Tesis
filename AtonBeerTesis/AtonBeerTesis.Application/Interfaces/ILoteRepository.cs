using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface ILoteRepository
    {
        // Métodos del PlanificacionService (HEAD)
        Task<Lote> CreateAsync(Lote lote);
        Task<Lote> GetByIdAsync(int id);
        Task<IEnumerable<RecetaInsumo>> GetRecetaInsumosByLoteIdAsync(int loteId);

        // Métodos del LoteService (Feature)
        Task<List<Lote>> GetAllAsync();
        Task<Lote?> GetActivoByFermentadorIdAsync(int fermentadorId);
        Task<bool> ExisteCodigoAsync(string codigo);
        Task<bool> UpdateAsync(Lote lote);
        Task DeleteByIdAsync(int id);
    }
}