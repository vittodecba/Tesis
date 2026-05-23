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
        private readonly IBarrilRepository _barrilRepository;
        private readonly IPedidoRepository _pedidoRepository;//ver

        public StockService(
            IRepository<ProductoStock> productoStockRepository,
            IMovimientoStockRepository movimientoStockRepository,
            IRepository<MovimientoStock> movimientoStockWriteRepository,
            IBarrilRepository barrilRepository,
            IPedidoRepository pedidoRepository)
        {
            _productoStockRepository = productoStockRepository;
            _movimientoStockRepository = movimientoStockRepository;
            _movimientoStockWriteRepository = movimientoStockWriteRepository;
            _barrilRepository = barrilRepository;
            _pedidoRepository = pedidoRepository;
        }

       public async Task<IEnumerable<ProductoStockDto>> ObtenerTodosAsync()
         {
             var productos = await _productoStockRepository.GetAllAsync("FormatoEnvase", "Receta");

             return productos.Select(p => new ProductoStockDto
             {
                 Id = p.Id,
                 Estilo = p.Estilo,
                 RecetaId = p.RecetaId,
                 RecetaNombre = p.Receta?.Nombre,
                 FormatoEnvaseNombre = p.FormatoEnvase?.Nombre ?? "Sin Formato",
                 CapacidadLitros = p.FormatoEnvase?.CapacidadLitros ?? 0,
                 EsRetornable = p.FormatoEnvase?.EsRetornable ?? false,
                 StockActual = p.StockActual,
                 StockDisponible = p.StockActual
             });
         }

        public async Task<IEnumerable<MovimientoDetalladoDto>> ObtenerMovimientosAsync()
        {
            return await _movimientoStockRepository.GetMovimientosDetalladosAsync();
        }

        public async Task<MovimientoDetalladoDto> AgregarIngresoManualAsync(CreateIngresoManualDto dto)
        {
            if (dto.Cantidad <= 0)
                throw new Exception("La cantidad debe ser mayor a 0");

            var producto = await _productoStockRepository.FindOneAsync(dto.ProductoStockId, "FormatoEnvase")
                ?? throw new Exception("Producto de stock no encontrado");

            // Validar barriles si el formato es retornable
            var formatosRetornables = await _barrilRepository.ObtenerFormatosRetornablesAsync();
            List<int>? idsALlenar = null;
            if (formatosRetornables.TryGetValue(producto.FormatoEnvaseId, out _))
            {
                int unidades = (int)dto.Cantidad;
                var disponibles = await _barrilRepository.GetDisponiblesAsync(
                    producto.FormatoEnvaseId, unidades);

                if (disponibles.Count < unidades)
                    throw new Exception(
                        $"No hay barriles suficientes: se necesitan {unidades} barril(es) " +
                        $"disponibles del formato y solo hay {disponibles.Count}. " +
                        "Registrá los barriles en el módulo de Barriles.");

                idsALlenar = disponibles.Select(b => b.Id).ToList();
            }

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

            // Marcar barriles como Llenos después de confirmar el stock
            if (idsALlenar != null)
                await _barrilRepository.MarcarComoLlenosAsync(idsALlenar, movimiento.LoteId.Value);

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
                FormatoNombre = formato?.Nombre ?? "Sin Formato",
                CapacidadLitros = formato?.CapacidadLitros ?? 0,
                LoteId = null
            };
        }

        public async Task<MovimientoDetalladoDto> CorregirStockAsync(int productoStockId, CorreccionStockDto dto)
        {
            if (dto.NuevaCantidad < 0)
                throw new Exception("El stock no puede ser negativo.");

            var producto = await _productoStockRepository.FindOneAsync(productoStockId, "FormatoEnvase")
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
                FormatoNombre = formato?.Nombre ?? "Sin Formato",
                CapacidadLitros = formato?.CapacidadLitros ?? 0,
                LoteId = null
            };
        }

        public async Task<MovimientoDetalladoDto> EgresoManualAsync(CreateIngresoManualDto dto)
        {
            if (dto.Cantidad <= 0)
                throw new Exception("La cantidad debe ser mayor a 0.");

            var producto = await _productoStockRepository.FindOneAsync(dto.ProductoStockId, "FormatoEnvase")
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
                FormatoNombre = formato?.Nombre ?? "Sin Formato",
                CapacidadLitros = formato?.CapacidadLitros ?? 0,
                LoteId = null
            };
        }
    }
}