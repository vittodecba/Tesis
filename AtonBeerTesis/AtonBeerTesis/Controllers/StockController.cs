using AtonBeerTesis.Application.Dtos.STOCK;
using AtonBeerTesis.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet("productos")]
        public async Task<IActionResult> GetAllProductos()
        {
            return Ok(await _stockService.ObtenerTodosAsync());
        }

        [HttpGet("movimientos")]
        public async Task<IActionResult> GetMovimientos()
        {
            return Ok(await _stockService.ObtenerMovimientosAsync());
        }

        [HttpPost("ingresos")]
        public async Task<IActionResult> AgregarIngreso([FromBody] CreateIngresoManualDto dto)
        {
            try
            {
                var resultado = await _stockService.AgregarIngresoManualAsync(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
