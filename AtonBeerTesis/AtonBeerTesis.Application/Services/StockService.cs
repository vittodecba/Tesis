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
        private readonly IRepository<MovimientoStock> _movimientoStockWriteRepository;

        public StockService(
            IRepository<ProductoStock> productoStockRepository,
            IMovimientoStockRepository movimientoStockRepository,
            IRepository<MovimientoStock> movimientoStockWriteRepository)
        {
            _productoStockRepository = productoStockRepository;
            _movimientoStockRepository = movimientoStockRepository;
            _movimientoStockWriteRepository = movimientoStockWriteRepository;
        }

        public async Task<IEnumerable<ProductoStock>> ObtenerTodosAsync()
        {
            return await _productoStockRepository.GetAllAsync();
        }

        public async Task<IEnumerable<MovimientoDetalladoDto>> ObtenerMovimientosAsync()
        {
            return await _movimientoStockRepository.GetMovimientosDetalladosAsync();
        }

        public async Task<MovimientoDetalladoDto> AgregarIngresoManualAsync(CreateIngresoManualDto dto)
        {
            if (dto.Cantidad <= 0)
                throw new Exception("La cantidad debe ser mayor a 0");

            var producto = await _productoStockRepository.FindOneAsync(dto.ProductoStockId)
                ?? throw new Exception("Producto de stock no encontrado");

            var formato = producto.FormatoEnvase;

            var stockPrevio = producto.StockActual;
            producto.StockActual += dto.Cantidad;

            var motivo = string.IsNullOrWhiteSpace(dto.Motivo) ? "Ingreso Manual" : dto.Motivo.Trim();

            var movimiento = new MovimientoStock
            {
                ProductoStockId = producto.Id,
                LoteId = null,
                Cantidad = dto.Cantidad,
                TipoMovimiento = "Ingreso",
                MotivoMovimiento = motivo,
                StockPrevio = stockPrevio,
                StockResultante = producto.StockActual,
                Fecha = DateTime.Now
            };

            await _movimientoStockWriteRepository.AddAsync(movimiento);

            return new MovimientoDetalladoDto
            {
                Id = movimiento.Id,
                Fecha = movimiento.Fecha,
                TipoMovimiento = movimiento.TipoMovimiento,
                MotivoMovimiento = movimiento.MotivoMovimiento,
                Cantidad = movimiento.Cantidad,
                StockPrevio = movimiento.StockPrevio,
                StockResultante = movimiento.StockResultante,
                Estilo = producto.Estilo,
                FormatoNombre = formato?.Nombre ?? "",
                CapacidadLitros = formato?.CapacidadLitros ?? 0,
                LoteId = null
            };
        }

        public async Task<MovimientoDetalladoDto> CorregirStockAsync(int productoStockId, CorreccionStockDto dto)
        {
            if (dto.NuevaCantidad < 0)
                throw new Exception("El stock no puede ser negativo.");

            var producto = await _productoStockRepository.FindOneAsync(productoStockId)
                ?? throw new Exception("Producto de stock no encontrado.");

            var formato = producto.FormatoEnvase;
            var stockPrevio = producto.StockActual;
            var diferencia = dto.NuevaCantidad - stockPrevio;

            producto.StockActual = dto.NuevaCantidad;

            var movimiento = new MovimientoStock
            {
                ProductoStockId = producto.Id,
                LoteId = null,
                Cantidad = Math.Abs(diferencia),
                TipoMovimiento = diferencia >= 0 ? "Ingreso" : "Egreso",
                MotivoMovimiento = "Corrección Manual",
                StockPrevio = stockPrevio,
                StockResultante = producto.StockActual,
                Fecha = DateTime.Now
            };

            await _movimientoStockWriteRepository.AddAsync(movimiento);

            return new MovimientoDetalladoDto
            {
                Id = movimiento.Id,
                Fecha = movimiento.Fecha,
                TipoMovimiento = movimiento.TipoMovimiento,
                MotivoMovimiento = movimiento.MotivoMovimiento,
                Cantidad = movimiento.Cantidad,
                StockPrevio = stockPrevio,
                StockResultante = producto.StockActual,
                Estilo = producto.Estilo,
                FormatoNombre = formato?.Nombre ?? "",
                CapacidadLitros = formato?.CapacidadLitros ?? 0,
                LoteId = null
            };
        }

        public async Task<MovimientoDetalladoDto> EgresoManualAsync(CreateIngresoManualDto dto)
        {
            if (dto.Cantidad <= 0)
                throw new Exception("La cantidad debe ser mayor a 0.");

            var producto = await _productoStockRepository.FindOneAsync(dto.ProductoStockId)
                ?? throw new Exception("Producto de stock no encontrado.");

            if (producto.StockActual < dto.Cantidad)
                throw new Exception($"Stock insuficiente. Stock actual: {producto.StockActual} u.");

            var formato = producto.FormatoEnvase;
            var stockPrevio = producto.StockActual;
            producto.StockActual -= dto.Cantidad;

            var motivo = string.IsNullOrWhiteSpace(dto.Motivo) ? "Egreso Manual" : dto.Motivo.Trim();

            var movimiento = new MovimientoStock
            {
                ProductoStockId = producto.Id,
                LoteId = null,
                Cantidad = dto.Cantidad,
                TipoMovimiento = "Egreso",
                MotivoMovimiento = motivo,
                StockPrevio = stockPrevio,
                StockResultante = producto.StockActual,
                Fecha = DateTime.Now
            };

            await _movimientoStockWriteRepository.AddAsync(movimiento);

            return new MovimientoDetalladoDto
            {
                Id = movimiento.Id,
                Fecha = movimiento.Fecha,
                TipoMovimiento = movimiento.TipoMovimiento,
                MotivoMovimiento = movimiento.MotivoMovimiento,
                Cantidad = movimiento.Cantidad,
                StockPrevio = stockPrevio,
                StockResultante = producto.StockActual,
                Estilo = producto.Estilo,
                FormatoNombre = formato?.Nombre ?? "",
                CapacidadLitros = formato?.CapacidadLitros ?? 0,
                LoteId = null
            };
        }
    }
}
