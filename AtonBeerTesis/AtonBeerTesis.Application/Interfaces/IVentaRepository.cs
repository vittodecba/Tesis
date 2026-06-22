using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IVentaRepository
    {
        Task<Venta> AddAsync(Venta venta);
        Task UpdateAsync(Venta venta);
        Task<IEnumerable<Venta>> GetAllAsync();
        Task<Venta?> GetByIdAsync(int id);
        Task<List<Venta>> GetVentasPorRangoAsync(DateTime fechaDesde, DateTime fechaHasta);
        IQueryable<Venta> GetQueryable();
        Task<Venta?> GetByPedidoIdAsync(int pedidoId);
        Task<bool> TieneClienteVentasImpagasAsync(int clienteId);
    }
}