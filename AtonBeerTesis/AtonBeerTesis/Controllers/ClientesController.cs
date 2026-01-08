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

        // GET: api/clientes?tipo=Externo&ubicacion=Cordoba&estado=Activo
        [HttpGet]
        public async Task<ActionResult<List<ClienteDto>>> GetAll([FromQuery] string? tipo, [FromQuery] string? ubicacion, [FromQuery] string? estado)
        {
            var result = await _clienteService.GetAllAsync(tipo, ubicacion, estado);
            return Ok(result);
        }

        // GET: api/clientes/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClienteDto>> GetById(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente is null) return NotFound();
            return Ok(cliente);
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CrearClienteDto dto)
        {
            var id = await _clienteService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // PUT: api/clientes/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] ActualizarClienteDto dto)
        {
            var ok = await _clienteService.UpdateAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        // DELETE (lógico): api/clientes/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Deactivate(int id)
        {
            var ok = await _clienteService.DeactivateAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // GET combos: api/clientes/catalogos/tipos
        [HttpGet("catalogos/tipos")]
        public ActionResult<List<string>> GetTipos() => Ok(_clienteService.GetTiposCliente());

        // GET combos: api/clientes/catalogos/estados
        [HttpGet("catalogos/estados")]
        public ActionResult<List<string>> GetEstados() => Ok(_clienteService.GetEstadosCliente());
    }
}
