using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Dtos.Recetas;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Application.Services;
using AtonBeerTesis.Domain.Entities;
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

        // --- ESTE ES EL ENDPOINT NUEVO PARA EL PBI 94 ---
        [HttpGet("{id:int}/detalle")]
        public async Task<IActionResult> GetDetalle(int id)
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
        [Tags("RecetasDetalle")]
        //Insumos a agregar en el detalle insumo por si se quiere agregar un insumo a una receta ya creada, sin necesidad de modificar toda la receta
        [HttpPost("{id}/insumos")]
        public async Task<IActionResult> AgregarInsumo(int id, [FromBody] RecetaInsumoDto dto)
        {
            // Lógica para insertar solo una fila en la tabla intermedia RecetaInsumos
            var resultado = await _recetaService.AddInsumoToReceta(id, dto);
            if (resultado) return Ok();
            return BadRequest("No se pudo agregar el insumo.");
        }
        [Tags("RecetasDetalle")]
        [HttpDelete("{id}/insumos/{insumoId}")]
        public async Task<IActionResult> EliminarInsumo(int id, int insumoId)
        {
            var resultado = await _recetaService.RemoveInsumoDeReceta(id, insumoId);
            if (resultado) return Ok();
            return BadRequest("No se pudo eliminar el insumo.");
        }

        // Agregar endpoints para manejar los pasos de elaboración 
        [Tags("PasosElaboracion")]
        // POST: api/Recetas/{id}/pasos
        [HttpPost("{id}/pasos")]
        public async Task<IActionResult> AgregarPaso(int id, [FromBody] PasosElaboracionDto dto) // <-- Usar DTO
        {
            var paso = new PasosElaboracion
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Temperatura = dto.Temperatura,
                Tiempo = dto.Tiempo,
                Orden = dto.Orden
                // RecetaId se asigna en el Service
            };

            var nuevoPaso = await _recetaService.CrearPasoAsync(id, paso);
            return Ok(nuevoPaso);
        }
        [Tags("PasosElaboracion")]
        // PUT: api/Recetas/{id}/pasos/{pasoId}
        [HttpPut("{id}/pasos/{pasoId}")]
        public async Task<IActionResult> EditarPaso(int id, int pasoId, [FromBody] PasosElaboracionDto dto) // <-- Cambiado a DTO
        {
            try
            {
                // Convertimos el DTO a la entidad para el Service
                var paso = new PasosElaboracion
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Temperatura = dto.Temperatura,
                    Tiempo = dto.Tiempo,
                    Orden = dto.Orden
                };

                var actualizado = await _recetaService.EditarPasoAsync(id, pasoId, paso);
                if (!actualizado) return NotFound(new { message = "No se pudo encontrar la receta o el paso." });

                return Ok(new { message = "Paso actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Tags("PasosElaboracion")]
        // DELETE: api/Recetas/{id}/pasos/{pasoId}
        [HttpDelete("{id}/pasos/{pasoId}")]
        public async Task<IActionResult> EliminarPaso(int id, int pasoId)
        {
            try
            {
                var eliminado = await _recetaService.EliminarPasoAsync(id, pasoId);
                if (!eliminado) return NotFound(new { message = "El paso no existe o no pudo ser eliminado." });

                return Ok(new { message = "Paso eliminado con éxito" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}