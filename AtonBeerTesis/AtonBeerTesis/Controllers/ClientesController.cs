using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Dtos.Cliente;
using AtonBeerTesis.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ClienteDto>>> GetAll([FromQuery] string? tipo, [FromQuery] string? ubicacion, [FromQuery] string? estado)
        {
            var result = await _clienteService.GetAllAsync(tipo, ubicacion, estado);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClienteDto>> GetById(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente is null) return NotFound();
            return Ok(cliente);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CrearClienteDto dto)
        {
            try
            {
                var id = await _clienteService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id }, new { id });
            }
            catch (Exception ex)
            {
                // Así el frontend recibe el mensaje "El CUIT ingresado no es válido"
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] ActualizarClienteDto dto)
        {
            try
            {
                var ok = await _clienteService.UpdateAsync(id, dto);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, [FromBody] PatchClienteDto dto)
        {
            try
            {
                var ok = await _clienteService.PatchAsync(id, dto);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Deactivate(int id)
        {
            var ok = await _clienteService.DeactivateAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpGet("catalogos/tipos")]
        public ActionResult<List<string>> GetTipos() => Ok(_clienteService.GetTiposCliente());

        [HttpGet("catalogos/estados")]
        public ActionResult<List<string>> GetEstados() => Ok(_clienteService.GetEstadosCliente());
    }
}