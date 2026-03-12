using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AtonBeerTesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanProduccionController : ControllerBase
    {
        private readonly IPlanificacionService _planificacionService;

        public PlanProduccionController(IPlanificacionService planificacionService)
        {
            _planificacionService = planificacionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta,
            [FromQuery] int? recetaId,
            [FromQuery] int? fermentadorId,
            [FromQuery] string? estado)
        {
            var lista = await _planificacionService.GetAllAsync();
            var query = lista.AsEnumerable();

            if (fechaDesde.HasValue)
                query = query.Where(x => x.FechaProduccion.Date >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                query = query.Where(x => x.FechaProduccion.Date <= fechaHasta.Value.Date);

            if (recetaId.HasValue && recetaId > 0)
                query = query.Where(x => x.RecetaId == recetaId.Value);

            if (fermentadorId.HasValue && fermentadorId > 0)
                query = query.Where(x => x.FermentadorId == fermentadorId.Value);

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(x => x.Estado != null && x.Estado.Contains(estado, StringComparison.OrdinalIgnoreCase));

            return Ok(query.ToList());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var lista = await _planificacionService.GetAllAsync();
            var lote = lista.FirstOrDefault(x => x.Id == id);

            if (lote == null) return NotFound(new { message = $"No se encontró el lote con ID {id}" });
            return Ok(lote);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlanificacionProduccionDto dto)
        {
            if (dto == null) return BadRequest("Datos no proporcionados.");
            try
            {
                var resultado = await _planificacionService.PLanificarProduccion(dto);
                return Ok(new { message = "Planificacion creada con exito", data = resultado });
            }
            catch (Exception ex)
            {
                // ESTA LÍNEA ES LA CLAVE: nos va a decir qué columna o qué dato está fallando de verdad
                var mensajeReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensajeReal });
            }
        }

        [HttpGet("{id}/insumos-combinados")]
        public async Task<IActionResult> GetInsumosCombinados(int id)
        {
            var lista = await _planificacionService.GetAllAsync();
            var lote = lista.FirstOrDefault(x => x.Id == id);
            if (lote == null) return NotFound("Lote no encontrado.");
            var insumos = await _planificacionService.GetInsumosCalculadosAsync(lote.RecetaId);
            return Ok(insumos);
        }

        [HttpPut("{id}/asignar-fermentador/{fermentadorId}")]
        public async Task<IActionResult> AsignarFermentador(int id, int fermentadorId)
        {
            try
            {
                await _planificacionService.AsignarFermentadorAsync(id, fermentadorId);
                return Ok(new { message = "Fermentador asignado correctamente" });
            }
            catch (Exception ex)
            {
                var mensajeReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensajeReal });
            }
        }
    }
}