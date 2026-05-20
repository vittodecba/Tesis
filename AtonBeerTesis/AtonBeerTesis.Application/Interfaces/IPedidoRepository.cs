using AtonBeerTesis.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IPedidoRepository
    {
        Task<IEnumerable<Pedido>> GetAllAsync();
        Task<Pedido> AddAsync(Pedido pedido);
        Task<Pedido?> GetByIdAsync(int id);
        Task UpdateAsync(Pedido pedido);
        ///Chequear-validaciones con el stock
        Task<ProductoStock?> GetProductoStockByIdAsync(int id);
        Task<decimal> ObtenerCantidadReservadaPendienteAsync(int productoStockId, int? pedidoIdExcluir = null);
        Task AgregarMovimientoStockAsync(MovimientoStock movimiento);
        Task<Dictionary<int, decimal>> ObtenerReservasPendientesPorProductoAsync();
    }
}