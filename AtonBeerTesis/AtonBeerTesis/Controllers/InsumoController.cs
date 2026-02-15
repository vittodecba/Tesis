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
        public async Task<ActionResult<IEnumerable<InsumoDto>>> GetInsumos()
        {
            var lista = await _context.Insumos
                .Include(i => i.TipoInsumo)
                .Include(i => i.unidadMedida)
                .Select(i => new InsumoDto
                {
                    Id = i.Id, // IMPORTANTE: Para que el borrado funcione en Angular
                    NombreInsumo = i.NombreInsumo,
                    Codigo = i.Codigo,
                    TipoInsumoId = i.TipoInsumoId,

                    // Ahora traemos el nombre REAL o avisamos que no hay
                    TipoNombre = i.TipoInsumo != null ? i.TipoInsumo.Nombre : "Sin Categoría",

                    unidadMedidaId = i.unidadMedidaId,

                    // Traemos la abreviatura REAL (Lt, Kg, etc.)
                    Unidad = i.unidadMedida != null ? i.unidadMedida.Abreviatura : "S/U",

                    StockActual = i.StockActual,
                    Observaciones = i.Observaciones,
                    UltimaActualizacion = i.UltimaActualizacion ?? DateTime.Now
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpPost]
        public async Task<IActionResult> CrearInsumo([FromBody] InsumoDto insumoDto)
        {
            // ... validaciones de null y existencia ...

            var nuevoInsumo = new Insumo
            {
                NombreInsumo = insumoDto.NombreInsumo,
                Codigo = "INS-" + (await _context.Insumos.CountAsync() + 1).ToString("000"),
                TipoInsumoId = insumoDto.TipoInsumoId,

                // FORZAMOS EL ID DIRECTAMENTE
                unidadMedidaId = insumoDto.unidadMedidaId,

                StockActual = insumoDto.StockActual,
                UltimaActualizacion = DateTime.Now,
                Observaciones = insumoDto.Observaciones,
                Activo = true
            };

            _context.Insumos.Add(nuevoInsumo);

            // ESTO ES LO NUEVO: Forzamos a EF a que reconozca que el ID ha cambiado
            _context.Entry(nuevoInsumo).Property(x => x.unidadMedidaId).IsModified = true;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Insumo creado con éxito" });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarInsumo(int id, [FromBody] InsumoDto insumoDto)
        {
            var insumo = await _context.Insumos.FindAsync(id);
            if (insumo == null) return NotFound();

            bool existe = await _context.Insumos.AnyAsync(x =>
                x.NombreInsumo == insumoDto.NombreInsumo &&
                x.TipoInsumoId == insumoDto.TipoInsumoId &&
                x.Id != id);

            if (existe) return BadRequest($"Ya existe otro insumo con ese nombre y tipo.");

            // Actualizamos los valores
            insumo.NombreInsumo = insumoDto.NombreInsumo;
            insumo.TipoInsumoId = insumoDto.TipoInsumoId;

            // CORRECCIÓN: Se corrigieron los errores CS1061 y CS0117
            // Usamos la propiedad del modelo real: UnidadMedidaId
            insumo.unidadMedidaId = insumoDto.unidadMedidaId;

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
            if (insumo == null) return NotFound(new { message = "El insumo no existe." });

            _context.Insumos.Remove(insumo);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Insumo eliminado correctamente." });
        }

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

        [HttpGet("buscar")]
        public async Task<IActionResult> GetInsumosFiltrados([FromQuery] string? nombre, [FromQuery] int? tipoId)
        {
            var query = _context.Insumos
                .Include(i => i.TipoInsumo)
                .Include(i => i.unidadMedida)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nombre))
                query = query.Where(i => i.NombreInsumo.Contains(nombre));

            if (tipoId.HasValue)
                query = query.Where(i => i.TipoInsumoId == tipoId.Value);

            return Ok(await query.ToListAsync());
        }
        // --- GESTIÓN DE TIPOS DE INSUMO  ---

        [HttpPost("tipos")]
        public async Task<IActionResult> CrearTipoInsumo([FromBody] TipoInsumo nuevoTipo)
        {
            if (nuevoTipo == null || string.IsNullOrEmpty(nuevoTipo.Nombre))
                return BadRequest("El nombre del tipo es obligatorio.");

            var existe = await _context.TiposInsumo.AnyAsync(t => t.Nombre == nuevoTipo.Nombre);
            if (existe) return BadRequest("Esta categoría ya existe.");

            _context.TiposInsumo.Add(nuevoTipo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tipo de insumo creado con éxito", id = nuevoTipo.id });
        }

        [HttpDelete("tipos/{id}")]
        public async Task<IActionResult> EliminarTipoInsumo(int id)
        {
            var tipo = await _context.TiposInsumo.FindAsync(id);
            if (tipo == null) return NotFound("El tipo no existe.");

            // Validación de seguridad: No borrar si hay insumos usándolo
            var enUso = await _context.Insumos.AnyAsync(i => i.TipoInsumoId == id);
            if (enUso) return BadRequest("No se puede eliminar: hay insumos vinculados a esta categoría.");

            _context.TiposInsumo.Remove(tipo);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Tipo eliminado correctamente." });
        }

        [HttpGet("tipos")]
        public async Task<ActionResult<IEnumerable<TipoInsumo>>> GetTipos()
        {
            return await _context.TiposInsumo.Where(t => t.Activo).ToListAsync();
        }

    }
}