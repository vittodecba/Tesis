using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Interfaces;

namespace AtonBeerTesis.Application.Services
{
    public class StockService : IStockService
    {
        private readonly IRepository<ProductoStock> _productoStockRepository;
        private readonly IRepository<MovimientoStock> _movimientoStockRepository;

        public StockService(
            IRepository<ProductoStock> productoStockRepository,
            IRepository<MovimientoStock> movimientoStockRepository)
        {
            _productoStockRepository = productoStockRepository;
            _movimientoStockRepository = movimientoStockRepository;
        }

        public async Task<IEnumerable<ProductoStock>> ObtenerTodosAsync()
        {
            return await _productoStockRepository.GetAllAsync();
        }

        public async Task<IEnumerable<MovimientoStock>> ObtenerMovimientosAsync()
        {
            return await _movimientoStockRepository.GetAllAsync();
        }
    }
}
