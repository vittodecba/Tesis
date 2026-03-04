using AtonBeerTesis.Application.DTOs;
using AtonBeerTesis.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FermentadorController : ControllerBase
    {
        private readonly IFermentadorService _service;

        public FermentadorController(IFermentadorService service)
        {
            _service = service;
        }

        // GET: api/fermentador
        [HttpGet]
        public async Task<ActionResult<List<FermentadorDto>>> GetAll()
        {
            var lista = await _service.GetAllAsync();
            return Ok(lista);
        }

        // POST: api/fermentador
        [HttpPost]
        public async Task<ActionResult<FermentadorDto>> Create(CreateFermentadorDto dto)
        {
            try
            {
                var resultado = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetAll), new { id = resultado.Id }, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}