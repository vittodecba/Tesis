using AtonBeerTesis.Application.Dtos.STOCK;
using AtonBeerTesis.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormatoEnvaseController : ControllerBase
    {
        private readonly IFormatoEnvaseService _service;

        public FormatoEnvaseController(IFormatoEnvaseService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.ObtenerTodosAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFormatoEnvaseDto dto)
        {
            try
            {
                var resultado = await _service.CrearFormatoAsync(dto);
                return CreatedAtAction(nameof(GetAll), new { id = resultado.Id }, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _service.EliminarFormatoAsync(id);
            if (!eliminado)
                return NotFound(new { mensaje = "Formato de envase no encontrado" });

            return NoContent();
        }
    }
}
