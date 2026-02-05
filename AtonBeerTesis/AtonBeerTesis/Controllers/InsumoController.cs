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
            return await _context.Insumos.Include(i => i.TipoInsumo)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> CrearInsumo([FromBody] InsumoDto insumoDto)
        {
            if (insumoDto == null) return BadRequest("Datos inválidos");
                
            bool existe = await _context.Insumos.AnyAsync(x => x.NombreInsumo == insumoDto.NombreInsumo && x.TipoInsumoId == insumoDto.TipoInsumoId);
            if (existe)
            {
                return BadRequest($"El insumo '{insumoDto.NombreInsumo}' de tipo '{insumoDto.TipoInsumoId}' ya existe.");
            }

            int cantidad = _context.Insumos.Count() + 1;
            string codigoAutomatico = "INS-" + cantidad.ToString("000");

            var nuevoInsumo = new Insumo
            {
                NombreInsumo = insumoDto.NombreInsumo,
                Codigo = codigoAutomatico,
                TipoInsumoId = insumoDto.TipoInsumoId,
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
                x.TipoInsumoId == insumoDto.TipoInsumoId &&
                x.Id != id);

            if (existe) return BadRequest($"Ya existe otro insumo con ese nombre y tipo.");

            // Actualizamos los valores
            insumo.NombreInsumo = insumoDto.NombreInsumo;
            insumo.TipoInsumoId = insumoDto.TipoInsumoId;
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

        //Agrego mi PBI 88 - RECLASIFICAR INSUMO
        [HttpPut("{idInsumo}/clasificar")]
        public async Task<IActionResult> ReClasificarInsumo(int idInsumo, [FromBody] int nuevoTipoId)
        {
            var insumo = await _context.Insumos.FindAsync(idInsumo);
            if (insumo == null || !insumo.Activo) return NotFound("Insumo no encontrado.");

            var tipoExiste = await _context.TiposInsumo.AnyAsync(t => t.id == nuevoTipoId && t.Activo);
            if (!tipoExiste) return BadRequest("Esa categoría no existe");

            insumo.TipoInsumoId = nuevoTipoId;
            await _context.SaveChangesAsync();
            return Ok("Insumo clasificado correctamente.");
        }

        //MÉTODO DE FILTROS 
        [HttpGet("buscar")]
        public async Task<IActionResult> GetInsumosFiltrados(
            [FromQuery] string? nombre,
            [FromQuery] int? tipoId)
        {
            var query = _context.Insumos
                .Include(i => i.TipoInsumo)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nombre))
                query = query.Where(i => i.NombreInsumo.Contains(nombre));

            if (tipoId.HasValue)
                query = query.Where(i => i.TipoInsumoId == tipoId.Value);

            return Ok(await query.ToListAsync());
        }

        //DE TIPOS (Para el dropdown del Front)
        [HttpGet("tipos")]
        public async Task<ActionResult<IEnumerable<TipoInsumo>>> GetTipos()
        {
            return await _context.TiposInsumo.Where(t => t.Activo).ToListAsync();
        }
    }
}