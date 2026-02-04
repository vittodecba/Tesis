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

            // --- VALIDACIÓN DE DUPLICADOS (NUEVO) ---
            // Verifica si ya existe en la base de datos un insumo con el mismo Nombre y Tipo
            bool existe = await _context.Insumos.AnyAsync(x => x.NombreInsumo == insumoDto.NombreInsumo && x.Tipo == insumoDto.Tipo);

            if (existe)
            {
                // Si existe, devolvemos error 400 y frenamos la ejecución
                return BadRequest($"El insumo '{insumoDto.NombreInsumo}' de tipo '{insumoDto.Tipo}' ya existe.");
            }
            // ----------------------------------------

            // LÓGICA DE CÓDIGO AUTOMÁTICO
            // Cuenta cuántos hay y le suma 1
            int cantidad = _context.Insumos.Count() + 1;

            // Genera el texto "INS-" seguido del número con 3 cifras (ej: INS-005)
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
    }
}