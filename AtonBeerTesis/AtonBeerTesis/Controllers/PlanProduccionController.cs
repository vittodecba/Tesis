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
            [FromQuery] int? recetaId,      // Cambiado a int
            [FromQuery] int? fermentadorId, // Cambiado a int
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlanificacionProduccionDto dto)
        {
            if (dto == null) return BadRequest("Datos de planificación no proporcionados.");

            try
            {
                var resultado = await _planificacionService.PLanificarProduccion(dto);
                return Ok(new { message = "Planificacion creada con exito", data = resultado });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}