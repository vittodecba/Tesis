using AtonBeerTesis.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturasController : ControllerBase
    {
        private readonly IFacturaService _facturaService;

        public FacturasController(IFacturaService facturaService)
        {
            _facturaService = facturaService;
        }

        // Genera la factura (comprobante no fiscal) para una venta entregada.
        [HttpPost("generar/{ventaId:int}")]
        public async Task<IActionResult> Generar(int ventaId)
        {
            try
            {
                var factura = await _facturaService.GenerarAsync(ventaId);
                return Ok(factura);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // Devuelve la factura asociada a una venta (o 404 si no existe).
        [HttpGet("venta/{ventaId:int}")]
        public async Task<IActionResult> ObtenerPorVenta(int ventaId)
        {
            var factura = await _facturaService.ObtenerPorVentaAsync(ventaId);
            if (factura is null) return NotFound();
            return Ok(factura);
        }

        // Descarga el PDF de la factura.
        [HttpGet("{id:int}/pdf")]
        public async Task<IActionResult> Pdf(int id)
        {
            try
            {
                var resultado = await _facturaService.ObtenerPdfAsync(id);
                if (resultado is null) return NotFound();
                return File(resultado.Value.Contenido, "application/pdf", resultado.Value.NombreArchivo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
