using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IBarrilRepository
    {
        Task<List<Barril>> GetAllAsync();
        Task<Barril?> GetByIdAsync(int id);
        Task<Barril> AddAsync(Barril barril);
        Task<bool> UpdateAsync(Barril barril);
        Task<bool> ExisteCodigoAsync(string codigo, int? excludeId = null);
    }
}
