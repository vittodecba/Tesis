using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IFacturaRepository
    {
        Task<Factura?> GetByIdAsync(int id);
        Task<Factura?> GetByVentaIdAsync(int ventaId);
        Task<int> GetMaxNumeroAsync(TipoComprobante tipo, int puntoVenta);
        // Mapa ventaId -> facturaId, para marcar en el listado de ventas cuáles ya están facturadas.
        Task<Dictionary<int, int>> GetFacturaIdsPorVentaAsync();
        Task<Factura> AddAsync(Factura factura);
        Task UpdateAsync(Factura factura);
    }
}
