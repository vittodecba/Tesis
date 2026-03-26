using AtonBeerTesis.Application.DTOs;
using AtonBeerTesis.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FermentadorController : ControllerBase
    {
        private readonly IFermentadorService _service;

        public FermentadorController(IFermentadorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<FermentadorDto>>> GetAll()
        {
            var lista = await _service.GetAllAsync();
            return Ok(lista);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFermentadorDto dto)
        {
            var resultado = await _service.UpdateAsync(id, dto);

            if (!resultado)
                return NotFound($"No se encontró el fermentador con ID {id}");

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<FermentadorDto>> Create(CreateFermentadorDto dto)
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
                var resultado = await _service.DeleteAsync(id);
                if (!resultado)
                    return NotFound($"No se encontró el fermentador con ID {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
