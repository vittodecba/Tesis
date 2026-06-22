using AtonBeerTesis.Application.Dtos.STOCK;
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
        private readonly ILoteDesignacionService _designacionService;

        public LoteController(ILoteService service, ILoteDesignacionService designacionService)
        {
            _service = service;
            _designacionService = designacionService;
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
            // Si el estado cambia a Finalizado o Descartado, usar el flujo completo
            if (!string.IsNullOrWhiteSpace(dto.Estado) &&
                (dto.Estado == "Finalizado" || dto.Estado == "Descartado"))
            {
                var estadoFinal = dto.Estado == "Finalizado"
                    ? EstadoLote.Finalizado
                    : EstadoLote.Descartado;
                try
                {
                    var okF = await _service.FinalizarAsync(id, estadoFinal);
                    if (!okF) return NotFound();
                    return Ok(new { mensaje = "Lote finalizado correctamente." });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            try
            {
                var ok = await _service.UpdateAsync(id, dto);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        // ── DESIGNACIONES DE VOLUMEN ──────────────────────────────────────

        [HttpGet("{id}/designaciones")]
        public async Task<IActionResult> GetDesignaciones(int id)
        {
            return Ok(await _designacionService.ObtenerPorLoteAsync(id));
        }

        [HttpPost("{id}/designaciones")]
        public async Task<IActionResult> AddDesignacion(int id, [FromBody] CreateLoteDesignacionDto dto)
        {
            try
            {
                var resultado = await _designacionService.AgregarDesignacionAsync(id, dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpDelete("{id}/designaciones/{desId}")]
        public async Task<IActionResult> DeleteDesignacion(int id, int desId)
        {
            var eliminado = await _designacionService.EliminarDesignacionAsync(desId);
            if (!eliminado)
                return NotFound(new { mensaje = "Designación no encontrada" });

            return NoContent();
        }
    }
}