using AtonBeerTesis.Application.Dtos.STOCK;
using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IStockService
    {
        Task<IEnumerable<ProductoStockDto>> ObtenerTodosAsync();
        Task<IEnumerable<MovimientoDetalladoDto>> ObtenerMovimientosAsync();
        Task<MovimientoDetalladoDto> AgregarIngresoManualAsync(CreateIngresoManualDto dto);
        Task<MovimientoDetalladoDto> CorregirStockAsync(int productoStockId, CorreccionStockDto dto);
        Task<MovimientoDetalladoDto> EgresoManualAsync(CreateIngresoManualDto dto);
    }
}
