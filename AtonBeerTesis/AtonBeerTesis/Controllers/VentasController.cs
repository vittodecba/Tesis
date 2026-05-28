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
    }
}
