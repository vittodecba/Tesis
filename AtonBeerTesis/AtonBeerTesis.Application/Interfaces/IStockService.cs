using AtonBeerTesis.Application.Dtos.STOCK;
using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IStockService
    {
        Task<IEnumerable<ProductoStock>> ObtenerTodosAsync();
        Task<IEnumerable<MovimientoDetalladoDto>> ObtenerMovimientosAsync();
    }
}
