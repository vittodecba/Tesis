using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.DTOs;
using AtonBeerTesis.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidosController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var pedidos = await _pedidoService.ObtenerTodosAsync();
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al obtener los pedidos", error = ex.Message });
            }
        }

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
        [HttpPut]
        public async Task<IActionResult> Put(int id,[FromBody] PedidoEdicionDTO pedidoDto)
        {
            try
            {
                if (id != pedidoDto.Id)
                {
                    return BadRequest(new { mensaje = "El ID del pedido no coincide con el ID proporcionado." });
                }
                var resultado = await _pedidoService.ActualizarPedidoAsync(pedidoDto);
                if (!resultado)
                {
                  return NotFound(new { mensaje = $"No se encontró el pedido con ID {id} para actualizar." });
                }            
               return Ok(new { mensaje = "¡Pedido actualizado con éxito en Aton Beer!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}