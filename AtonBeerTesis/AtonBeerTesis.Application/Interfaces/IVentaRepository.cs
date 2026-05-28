using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IVentaRepository
    {
        Task<Venta> AddAsync(Venta venta);
        Task UpdateAsync(Venta venta);
        Task<IEnumerable<Venta>> GetAllAsync();
    }
}
