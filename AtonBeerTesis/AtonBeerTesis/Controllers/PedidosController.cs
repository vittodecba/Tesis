using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.DTOs; // Verificá que este sea el namespace de tus DTOs
using AtonBeerTesis.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        // Inyectamos el servicio que creamos antes
        public PedidosController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        // POST api/Pedidos
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PedidoCreacionDTO pedidoDto)
        {
            try
            {
                if (pedidoDto == null)
                {
                    return BadRequest(new { mensaje = "Los datos del pedido son inválidos." });
                }

                var id = await _pedidoService.RegistrarPedidoAsync(pedidoDto);

                return Ok(new
                {
                    idPedido = id,
                    mensaje = "¡Pedido registrado con éxito en Aton Beer!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al registrar el pedido",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
    }
}