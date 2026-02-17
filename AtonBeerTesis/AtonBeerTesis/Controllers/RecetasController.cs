using AtonBeerTesis.Application.Dtos.Recetas;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecetasController : ControllerBase
    {
        private readonly IRecetaService _recetaService;

        public RecetasController(IRecetaService recetaService)
        {
            _recetaService = recetaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
     [FromQuery] string? nombre = null,
     [FromQuery] string? estilo = null,
     [FromQuery] string? estado = null,
     [FromQuery] string? orden = "fecha_desc") // Por defecto, las más nuevas primero
        {
            var result = await _recetaService.GetAllAsync(nombre, estilo, estado, orden);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _recetaService.GetByIdAsync(id);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRecetaDto dto)
        {
            var id = await _recetaService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ActualizarRecetaDto dto)
        {
            var ok = await _recetaService.UpdateAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, [FromBody] PatchRecetaDto dto)
        {
            var ok = await _recetaService.PatchAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPut("{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var ok = await _recetaService.DeactivateAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpGet("estados")]
        public IActionResult GetEstados()
        {
            var estados = _recetaService.GetEstadosReceta();
            return Ok(estados);
        }
    }
}
