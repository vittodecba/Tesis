using AtonBeerTesis.Application.Dto;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using AtonBeerTesis.Domain.Interfaces;

namespace AtonBeerTesis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly IRepository<MovimientoStock> _movimientoRepository;
        public StockController(IStockService stockService, IRepository<MovimientoStock> movimientoRepository)
        {
            _stockService = stockService;
            _movimientoRepository = movimientoRepository;
        }
        [HttpDelete("productos/{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            // Llamamos al servicio que ya tiene la lógica de borrar movimientos y producto
            var resultado = await _stockService.EliminarProductoAsync(id);

            if (!resultado)
            {
                // Si el servicio devuelve false es porque no encontró el producto
                return NotFound(new { message = "No se encontró el producto para eliminar." });
            }

            // Si todo sale bien, devolvemos un 204 (No Content) que es el estándar para DELETE
            return NoContent();
        }

        [HttpGet("productos")]
        public async Task<IActionResult> GetAllProductos()
        {
            return Ok(await _stockService.ObtenerTodosAsync());
        }

        [HttpPost("productos")]
        public async Task<IActionResult> CrearProducto([FromBody] ProductoDto dto)
        {
            await _stockService.CrearProductoAsync(dto);
            return Ok(new { mensaje = "Producto creado exitosamente en el catálogo." });
        }

        [HttpPut("productos/{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] ProductoDto dto)
        {
            try
            {
                await _stockService.ActualizarProductoAsync(id, dto);
                return Ok(new { mensaje = "Producto actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarMovimientoStock([FromBody] MovimientoStockDto dto)
        {
            try
            {
                var resultado = await _stockService.RegistrarMovimientoStockAsync(dto);
                if (resultado)
                {
                    return Ok(new { mensaje = "Movimiento de stock registrado exitosamente." });
                }
                else
                {
                    return BadRequest(new { mensaje = "No se pudo registrar el movimiento de stock." });
                }
            }
            catch (InvalidOperationException ex)
            {
                //Aca capturamos errores específicos de validación que vienen de la lógica del servicio
                return BadRequest(new { mensaje = ex.Message });
            }
            catch(Exception ex)
            {
                //Errores generales
                return StatusCode(500, new { mensaje = "Error interno del servidor: " + ex.Message });
            }
        }
        [HttpGet("movimientos")]
        public async Task<IActionResult> GetMovimientos()
        {
            var historial = await _stockService.ObtenerHistorialConNombresAsync();
            return Ok(historial);
        }
    }
}
