using AtonBeerTesis.Application.Dto;
using AtonBeerTesis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IStockService
    {
        Task<bool>RegistrarMovimientoStockAsync(MovimientoStockDto dto);
        Task<IEnumerable<ProductoPrueba>> ObtenerTodosAsync();
        Task<ProductoPrueba> ObtenerPorIdAsync(int id);
        Task<bool> CrearProductoAsync(ProductoDto dto);
        Task<bool> ActualizarProductoAsync(int id, ProductoDto dto);
    }
}
