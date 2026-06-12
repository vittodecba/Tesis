using AtonBeerTesis.Application.Dtos.EMPRESA;
using AtonBeerTesis.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresaController : ControllerBase
    {
        private readonly IEmpresaService _empresaService;

        public EmpresaController(IEmpresaService empresaService)
        {
            _empresaService = empresaService;
        }

        // Datos del emisor (la fábrica). Una sola fila de configuración.
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var empresa = await _empresaService.ObtenerAsync();
            if (empresa is null)
                return NotFound(new { mensaje = "No hay datos de empresa configurados." });
            return Ok(empresa);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] ActualizarEmpresaDto dto)
        {
            try
            {
                var ok = await _empresaService.ActualizarAsync(dto);
                if (!ok) return NotFound(new { mensaje = "No hay datos de empresa configurados." });
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
