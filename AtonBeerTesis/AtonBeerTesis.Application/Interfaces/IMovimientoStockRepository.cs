using AtonBeerTesis.Application.Dtos.STOCK;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IMovimientoStockRepository
    {
        Task<IEnumerable<MovimientoDetalladoDto>> GetMovimientosDetalladosAsync();
    }
}
