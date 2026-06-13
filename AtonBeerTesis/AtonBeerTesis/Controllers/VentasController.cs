using AtonBeerTesis.Application.Dtos.VENTAS;
using AtonBeerTesis.Application.DTOs;
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

            [HttpGet("reporte")]
        public async Task<ActionResult<ReporteVentasDto>> ObtenerReporte([FromQuery] DateTime fechaDesde, [FromQuery] DateTime fechaHasta, [FromQuery] string? cliente = null)
        {
            try
            {
                var reporte = await _ventaService.ObtenerReporteVentasAsync(fechaDesde, fechaHasta, cliente);
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reporte/pdf")]
        public async Task<IActionResult> DescargarPdfReporte([FromBody] ReportePdfRequestDto request)
        {
            try
            {
                var pdfBytes = await _ventaService.GenerarPdfReporteVentasAsync(request);
                var nombreArchivo = $"Reporte_{request.TipoReporte.ToUpper()}_{request.FechaDesde:yyyyMMdd}_al_{request.FechaHasta:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", nombreArchivo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar el PDF: {ex.Message}");
            }
        }
    }
}