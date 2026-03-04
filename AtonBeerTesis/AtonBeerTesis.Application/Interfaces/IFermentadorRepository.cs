using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IFermentadorRepository
    {
        // Estos son los métodos que faltan y por los que te grita el error:
        Task<List<Fermentador>> GetAllAsync();
        Task<Fermentador> AddAsync(Fermentador fermentador);

        // Estos otros también te van a servir:
        Task<Fermentador?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Fermentador fermentador);
    }
}