using AtonBeerTesis.Application.DTOs;
using AtonBeerTesis.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroFermentacionController : ControllerBase
    {
        private readonly IRegistroFermentacionService _service;

        public RegistroFermentacionController(IRegistroFermentacionService service)
        {
            _service = service;
        }

        [HttpGet("lote/{loteId}")]
        public async Task<ActionResult<List<RegistroFermentacionDto>>> GetByLote(int loteId)
        {
            var lista = await _service.GetByLoteIdAsync(loteId);
            return Ok(lista);
        }

        [HttpPost]
        public async Task<ActionResult<RegistroFermentacionDto>> Create([FromBody] CreateRegistroFermentacionDto dto)
        {
            try
            {
                var resultado = await _service.CreateAsync(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                var errorReal = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(errorReal);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRegistroFermentacionDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();

            return NoContent();
        }
    }
}