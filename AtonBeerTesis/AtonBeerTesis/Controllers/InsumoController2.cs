using Microsoft.AspNetCore.Mvc;
using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Dto;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsumoController2 : BaseController//Lo hago asi hasta que me pasen el codigo correcto del insumo
    {
        //Primero inyecto el contexto para poder usar la base de datos
        private readonly ApplicationDbContext _context;
        public InsumoController2(ApplicationDbContext context)
        {
            _context = context;
        }
        //Post para probar la creacion de un insumo hasta que me pasen el codigo correcto
        [HttpPost("CreacionInsumoPrueba")]
        public async Task<IActionResult> CreateInsumo(InsumoCreateDto dto)
        {
            // Verificar si el TipoInsumo existe y está activo
            var tipoExiste = await _context.TiposInsumo.AnyAsync(t => t.id == dto.TipoInsumoId && t.Activo);
            if (tipoExiste == false) return BadRequest("El tipo de insumo seleccionado no es valido o no existe");
            var nuevoInsumo = new Insumo
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                TipoInsumoId = dto.TipoInsumoId,
                Activo = true
            };
            _context.Insumos.Add(nuevoInsumo);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("Tipos")]
        public async Task<ActionResult<IEnumerable<TipoInsumo>>> GetTipos()
        {
            // Esto le da al Front la lista: {id: 1, nombre: "Malta"}, {id: 2, nombre: "Lúpulo"}...
            return await _context.TiposInsumo
                .Where(t => t.Activo)
                .ToListAsync();
        }
        //Mi PBI
        [HttpPut("{idInsumo}/clasificar")]
        public async Task<IActionResult> ReClasificarInsumo(int idInsumo, [FromBody] int nuevoTipoId)
        {
            var insumo = await _context.Insumos.FindAsync(idInsumo);
            if (insumo == null || !insumo.Activo)
            {
                return NotFound("Insumo no encontrado o inactivo.");
            }
            var tipoExiste = await _context.TiposInsumo.AnyAsync(t => t.id == nuevoTipoId && t.Activo);
            if (!tipoExiste)
            {
                return BadRequest("Esa categoria no existe");
            }
            insumo.TipoInsumoId = nuevoTipoId;
            await _context.SaveChangesAsync();
            return Ok("Insumo clasificado correctamente.");
        }
        //Otro get para probar 
        [HttpGet("MostrarDatos")]
        public async Task<IActionResult> GetInsumos()
        {
            var insumos = await _context.Insumos
                .Include(i => i.TipoInsumo)
                .ToListAsync();
            return Ok(insumos);
        }
        [HttpGet("Filtros")]
        public async Task<IActionResult> GetInsumosFiltrados(
         [FromQuery] string? nombre,
         [FromQuery] int? tipoId,
         [FromQuery] int? unidadId)
        {
            // 1. Empezamos con la consulta base incluyendo las relaciones
            var query = _context.Insumos
                .Include(i => i.TipoInsumo)
                /*.Include(i => i.unidad)*/
                .AsQueryable();

            // 2. Filtro por Nombre (si el usuario escribió algo)
            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(i => i.Nombre.Contains(nombre));
            }

            // 3. Filtro por Tipo (Criterio de clasificación)
            if (tipoId.HasValue)
            {
                query = query.Where(i => i.TipoInsumoId == tipoId.Value);
            }

            // 4. Filtro por Unidad de Medida
            /*if (unidadId.HasValue)
            {
                query = query.Where(i => i.unidadMedidaId == unidadId.Value);
            }*/

            // 5. Ejecutamos la búsqueda en la base de datos
            var resultados = await query.ToListAsync();

            return Ok(resultados);
        }

    }
}   
