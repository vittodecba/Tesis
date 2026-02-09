using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IRecetaRepository
    {
        Task<List<Receta>> GetAllAsync();
        Task<Receta?> GetByIdAsync(int id);
        Task AddAsync(Receta receta);
        Task UpdateAsync(Receta receta);
    }
}
