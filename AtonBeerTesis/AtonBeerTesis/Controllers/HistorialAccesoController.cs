using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialAccesoController : ControllerBase
    {
        private readonly IHistorialAccesoRepository _historialRepository;

        public HistorialAccesoController(IHistorialAccesoRepository historialRepository)
        {
            _historialRepository = historialRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? email, [FromQuery] DateTime? fecha, [FromQuery] bool? exito)
        {
            var historial = await _historialRepository.ObtenerHistorialAsync(email, fecha, exito);

            var resultado = historial.Select(h => new HistorialAccesoDto
            {
                Id = h.Id,
                Usuario = h.Usuario != null ? h.Usuario.Nombre : "No identificado",
                Email = h.EmailIntentado,
                Fecha = h.FechaIntento,
                Exitoso = h.Exitoso,
                Detalles = h.Detalles,
                Ip = h.Ip
            });

            return Ok(resultado);
        }
    }
}