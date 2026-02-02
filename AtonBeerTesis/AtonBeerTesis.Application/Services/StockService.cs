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
                StockResultante = producto.StockActual
            };
            //Persistir cambios usando los repositorios
             _productoRepository.Update(producto.id, producto);
            await _movimientoStockRepository.AddAsync(movimiento);
            return true;
        }
    }
}
