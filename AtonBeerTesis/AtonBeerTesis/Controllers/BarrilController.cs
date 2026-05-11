using AtonBeerTesis.Application.Dtos.BARRIL;
using AtonBeerTesis.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarrilController : ControllerBase
    {
        private readonly IBarrilService _service;

        public BarrilController(IBarrilService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<BarrilDto>>> GetAll()
        {
            var lista = await _service.GetAllAsync();
            return Ok(lista);
        }

        [HttpPost]
        public async Task<ActionResult<BarrilDto>> Create([FromBody] CreateBarrilDto dto)
        {
            try
            {
                var resultado = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetAll), new { id = resultado.Id }, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var resultado = await _service.EliminarAsync(id);
                if (!resultado)
                    return NotFound($"No se encontró el barril con ID {id}.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBarrilDto dto)
        {
            try
            {
                var resultado = await _service.UpdateAsync(id, dto);
                if (!resultado)
                    return NotFound($"No se encontró el barril con ID {id}.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
