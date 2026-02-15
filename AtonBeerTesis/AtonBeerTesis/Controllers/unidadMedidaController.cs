using AtonBeerTesis.Application.Dto;
using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class unidadMedidaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;//Uso el contexto de la base de datos, en vez de un servicio porque es algo más simple
        public unidadMedidaController(ApplicationDbContext context)
        {
           _context = context;
        }
        //Crear una nueva unidad de medida
        [HttpPost]
        public async Task<IActionResult> Create(unidadMedidaDto dto)
        {
            //Criterio de aceptacion 1: No duplicar nombres
            var existente = await _context.unidadMedida.AnyAsync(u => u.Nombre.ToLower() == dto.Nombre.ToLower());
            if (existente) return BadRequest("Ya existe una unidad de medida con ese nombre");
            var nuevaUnidad = new unidadMedida
            {
                Nombre = dto.Nombre,
                Abreviatura = dto.Abreviatura
            };
            _context.unidadMedida.Add(nuevaUnidad);
            await _context.SaveChangesAsync();
            return Ok();
        }
        //Actualizar una unidad de medida
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, unidadMedidaDto dto)
        {
            var unidad = await _context.unidadMedida.FindAsync(id);//Busco la unidad por id
            if (unidad == null) return NotFound("No se encontró la unidad de medida");
            //Validar que el nuevo nombre no lo tenga otra unidad
            var duplicado = await _context.unidadMedida.AnyAsync(u => u.id != id && u.Nombre.ToLower() == dto.Nombre.ToLower());
            if (duplicado) return BadRequest("Ya existe otra unidad de medida con ese nombre");
            //Actualizar los datos
            unidad.Nombre = dto.Nombre;
            unidad.Abreviatura = dto.Abreviatura;
            await _context.SaveChangesAsync();
            return NoContent();//Devuelvo NoContent porque no es necesario devolver nada
        }
        //Obtener todas las unidades de medida
        [HttpGet]
        public async Task<ActionResult<IEnumerable<unidadMedida>>> Get()//IEnumerable para devolver una lista
        {            
           return await _context.unidadMedida.Where(u => u.Activo).ToListAsync();
        }
        //Eliminar una unidad de medida (desactivarla)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var unidad = await _context.unidadMedida.FindAsync(id);
            if (unidad == null) return NotFound("No se encontró la unidad de medida");
            unidad.Activo = false;//Desactivo la unidad en vez de eliminarla
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
