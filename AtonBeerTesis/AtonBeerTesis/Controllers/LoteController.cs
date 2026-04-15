using AtonBeerTesis.Application.DTOs;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoteController : ControllerBase
    {
        private readonly ILoteService _service;

        public LoteController(ILoteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<LoteDto>>> GetAll()
        {
            var lista = await _service.GetAllAsync();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LoteDetalleDto>> GetById(int id)
        {
            var lote = await _service.GetByIdAsync(id);
            if (lote == null) return NotFound();

            return Ok(lote);
        }

        [HttpGet("activo/fermentador/{fermentadorId}")]
        public async Task<ActionResult<LoteDto>> GetActivoByFermentadorId(int fermentadorId)
        {
            var lote = await _service.GetActivoByFermentadorIdAsync(fermentadorId);
            if (lote == null) return NotFound();

            return Ok(lote);
        }

        [HttpPost]
        public async Task<ActionResult<LoteDto>> Create([FromBody] CreateLoteDto dto)
        {
            try
            {
                var resultado = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
            }
            catch (Exception ex)
            {
                var errorReal = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(errorReal);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLoteDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok) return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/finalizar")]
        public async Task<IActionResult> Finalizar(int id, [FromBody] FinalizarLoteDto? dto)
        {
            var estadoFinal = dto?.Estado ?? EstadoLote.Finalizado;
            try
            {
                var ok = await _service.FinalizarAsync(id, estadoFinal);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}