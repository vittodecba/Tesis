using AtonBeerTesis.Application.Dtos.Pagos;
using AtonBeerTesis.Application.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        private readonly IPagoService _pagoService;

        public PagosController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] RegistrarPagoDto dto)
        {
            try
            {
                var pago = await _pagoService.RegistrarPagoAsync(dto);
                return Ok(pago);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("venta/{ventaId}")]
        public async Task<IActionResult> ObtenerPorVenta(int ventaId)
        {
            try
            {
                var pagos = await _pagoService.ObtenerPorVentaAsync(ventaId);
                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
