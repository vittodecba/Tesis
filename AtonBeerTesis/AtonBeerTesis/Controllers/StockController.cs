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

        [HttpPut("productos/{id}/correccion")]
        public async Task<IActionResult> CorregirStock(int id, [FromBody] CorreccionStockDto dto)
        {
            try
            {
                var resultado = await _stockService.CorregirStockAsync(id, dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("egresos")]
        public async Task<IActionResult> AgregarEgreso([FromBody] CreateIngresoManualDto dto)
        {
            try
            {
                var resultado = await _stockService.EgresoManualAsync(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}