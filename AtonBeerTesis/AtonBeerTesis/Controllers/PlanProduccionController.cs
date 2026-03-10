using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Getll()
        {
           var lista = await _planificacionService.GetAllAsync();
            return Ok(lista);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlanificacionProduccionDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Datos de planificación no proporcionados.");
            }
            try
            {
                var resultado = await _planificacionService.PLanificarProduccion(dto);
                return Ok(new{message = "Planificacion creada con exito", data = resultado});
            }
            catch (Exception ex)
            {
                return BadRequest(new{message= ex.Message});
            }
        }
    }
}

