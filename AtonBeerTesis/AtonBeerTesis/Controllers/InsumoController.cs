using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AtonBeerTesis.Infrastructure.Data;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Application.DTOs;
using AtonBeerTesis.Domain;
using AtonBeerTesis.Application;

namespace AtonBeerTesis.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsumoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InsumoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Insumo>>> GetInsumos()
        {
            return await _context.Insumos.ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> CrearInsumo([FromBody] InsumoDto insumoDto)
        {
            if (insumoDto == null) return BadRequest("Datos inválidos");

            bool existe = await _context.Insumos.AnyAsync(x => x.NombreInsumo == insumoDto.NombreInsumo && x.Tipo == insumoDto.Tipo);
            if (existe)
            {
                return BadRequest($"El insumo '{insumoDto.NombreInsumo}' de tipo '{insumoDto.Tipo}' ya existe.");
            }

            int cantidad = _context.Insumos.Count() + 1;
            string codigoAutomatico = "INS-" + cantidad.ToString("000");

            var nuevoInsumo = new Insumo
            {
                NombreInsumo = insumoDto.NombreInsumo,
                Codigo = codigoAutomatico,
                Tipo = insumoDto.Tipo,
                Unidad = insumoDto.Unidad,
                StockActual = insumoDto.StockActual,
                UltimaActualizacion = DateTime.Now,
                Observaciones = insumoDto.Observaciones
            };

            _context.Insumos.Add(nuevoInsumo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Insumo creado con éxito" });
        }

        // --- NUEVO MÉTODO PARA MODIFICAR (PUT) ---
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarInsumo(int id, [FromBody] InsumoDto insumoDto)
        {
            var insumo = await _context.Insumos.FindAsync(id);
            if (insumo == null) return NotFound();

            // Validación: Que no exista OTRO insumo con mismo nombre y tipo
            bool existe = await _context.Insumos.AnyAsync(x =>
                x.NombreInsumo == insumoDto.NombreInsumo &&
                x.Tipo == insumoDto.Tipo &&
                x.Id != id);

            if (existe) return BadRequest($"Ya existe otro insumo con ese nombre y tipo.");

            // Actualizamos los valores
            insumo.NombreInsumo = insumoDto.NombreInsumo;
            insumo.Tipo = insumoDto.Tipo;
            insumo.Unidad = insumoDto.Unidad;
            insumo.StockActual = insumoDto.StockActual;
            insumo.Observaciones = insumoDto.Observaciones;
            insumo.UltimaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Insumo actualizado con éxito" });


        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarInsumo(int id)
        {
            var insumo = await _context.Insumos.FindAsync(id);

            if (insumo == null)
            {
                return NotFound(new { message = "El insumo no existe." });
            }

            _context.Insumos.Remove(insumo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Insumo eliminado correctamente." });
        }
    }
}