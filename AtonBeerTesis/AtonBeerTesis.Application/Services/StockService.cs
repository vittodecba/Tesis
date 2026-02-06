using AtonBeerTesis.Application.Dto;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtonBeerTesis.Domain.Interfaces;


namespace AtonBeerTesis.Application.Services
{
    public class StockService : IStockService
    {
        private readonly IRepository<ProductoPrueba> _productoRepository;
        private readonly IRepository<MovimientoStock> _movimientoStockRepository;

        public StockService(IRepository<ProductoPrueba> productoRepository, IRepository<MovimientoStock> movimientoStockRepository)
        {
            _movimientoStockRepository = movimientoStockRepository;
            _productoRepository = productoRepository;
        }
        public async Task<IEnumerable<ProductoPrueba>> ObtenerTodosAsync()
        {
            return await _productoRepository.GetAllAsync();
        }

        public async Task<ProductoPrueba> ObtenerPorIdAsync(int id)
        {
            return await _productoRepository.FindOneAsync(id);
        }

        public async Task<bool> CrearProductoAsync(ProductoDto dto)
        {
            var nuevoProducto = new ProductoPrueba
            {
                Nombre = dto.Nombre,
                Estilo = dto.Estilo,
                Formato = dto.Formato,
                UnidadMedida = dto.UnidadMedida,
                StockActual = 0 // Todo producto nuevo nace con stock cero
            };
            await _productoRepository.AddAsync(nuevoProducto);
            return true;
        }
        public async Task<IEnumerable<MovimientoDetalladoDto>> ObtenerHistorialConNombresAsync()
        {
            var movimientos = await _movimientoStockRepository.GetAllAsync();
            var productos = await _productoRepository.GetAllAsync();

            // Hacemos un "Join" manual en memoria para traer los nombres
            return movimientos.Select(m => new MovimientoDetalladoDto
            {
                Fecha = m.Fecha,
                TipoMovimiento = m.TipoMovimiento,
                Cantidad = m.Cantidad,
                Motivo = m.MotivoMovimiento,
                ProductoNombre = productos.FirstOrDefault(p => p.id == m.ProductoId)?.Nombre ?? "Producto Desconocido"
            }).OrderByDescending(x => x.Fecha);
        }

        public async Task<bool> ActualizarProductoAsync(int id, ProductoDto dto)
        {
            var productoExistente = await _productoRepository.FindOneAsync(id);
            if (productoExistente == null) throw new Exception("Producto no encontrado");

            productoExistente.Nombre = dto.Nombre;
            productoExistente.Estilo = dto.Estilo;
            productoExistente.Formato = dto.Formato;
            productoExistente.UnidadMedida = dto.UnidadMedida;

            _productoRepository.Update(id, productoExistente);
            return true;
        }
        public async Task<bool> EliminarProductoAsync(int id)
        {
            // 1. Verificar si el producto existe
            var producto = await _productoRepository.FindOneAsync(id);
            if (producto == null) return false;

            // 2. Limpiar historial de movimientos
            // Usamos FindAllAsync para obtener la lista y filtramos
            var todosLosMovimientos = await _movimientoStockRepository.FindAllAsync();
            var movimientosDelProducto = todosLosMovimientos.Where(m => m.ProductoId == id).ToList();

            foreach (var mov in movimientosDelProducto)
            {
                // Tu repositorio usa Remove(params object[] keyValues) y guarda solo
                _movimientoStockRepository.Remove(mov.Id);
            }

            // 3. Eliminar el producto
            _productoRepository.Remove(id);

            return true;
        }
        public async Task<bool> RegistrarMovimientoStockAsync(MovimientoStockDto dto)
        {
            //Obtener el producto
            var producto = await _productoRepository.FindOneAsync(dto.productoId);
            if (producto == null)
            {
                throw new Exception("Producto no encontrado");
            }
            //Logica 1: Validar stock negativo o sea si es una salida
            if (dto.tipoMovimiento == "Egreso" && producto.StockActual < dto.cantidad)
            {
                throw new Exception("Stock insuficiente para realizar el egreso");
            }
            //Logica 2: Actualizar el stock del producto
            if (dto.tipoMovimiento == "Ingreso")
            {
                producto.StockActual += dto.cantidad;
            }
            else if (dto.tipoMovimiento == "Egreso")
            {
                producto.StockActual -= dto.cantidad;
            }
            //Logica 3: Registrar el movimiento de stock
            var movimiento = new MovimientoStock
            {
                ProductoId = dto.productoId,
                Cantidad = dto.cantidad,
                TipoMovimiento = dto.tipoMovimiento!,
                MotivoMovimiento = dto.motivoMovimiento!,
                StockPrevio = producto.StockActual,
                StockResultante = producto.StockActual,
                Fecha = DateTime.Now,
            };
            //Persistir cambios usando los repositorios
             _productoRepository.Update(producto.id, producto);
            await _movimientoStockRepository.AddAsync(movimiento);
            return true;
        }
    }
}
