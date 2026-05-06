using AtonBeerTesis.Application.Dtos.STOCK;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Interfaces;

namespace AtonBeerTesis.Application.Services
{
    public class StockService : IStockService
    {
        private readonly IRepository<ProductoStock> _productoStockRepository;
        private readonly IMovimientoStockRepository _movimientoStockRepository;

        public StockService(
            IRepository<ProductoStock> productoStockRepository,
            IMovimientoStockRepository movimientoStockRepository)
        {
            _productoStockRepository = productoStockRepository;
            _movimientoStockRepository = movimientoStockRepository;
        }

        public async Task<IEnumerable<ProductoStock>> ObtenerTodosAsync()
        {
            return await _productoStockRepository.GetAllAsync();
        }

        public async Task<IEnumerable<MovimientoDetalladoDto>> ObtenerMovimientosAsync()
        {
            return await _movimientoStockRepository.GetMovimientosDetalladosAsync();
        }
    }
}
