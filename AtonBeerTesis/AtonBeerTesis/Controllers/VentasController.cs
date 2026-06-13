using AtonBeerTesis.Application.Dtos.VENTAS;
using AtonBeerTesis.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentasController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var ventas = await _ventaService.ObtenerTodasAsync();
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al obtener las ventas", error = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] PatchVentaDto dto)
        {
            try
            {
                var ok = await _ventaService.PatchAsync(id, dto);
                if (!ok) return NotFound(new { mensaje = $"No se encontró la venta con ID {id}." });
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        [HttpPatch("{id}/descuento")]
        public async Task<IActionResult> AplicarDescuento(int id, [FromBody] AplicarDescuentoDto dto)
        {
            try
            {
                var ok = await _ventaService.AplicarDescuentoAsync(id, dto);
                if (!ok) return NotFound(new { mensaje = $"No se encontró la venta con ID {id}." });

                return Ok(new { mensaje = "Descuento aplicado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
