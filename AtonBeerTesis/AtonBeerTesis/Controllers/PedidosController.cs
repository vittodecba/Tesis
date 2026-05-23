using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.DTOs;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            try
            {
                var pedidoDto = await _pedidoService.ObtenerPorIdAsync(id);
                if (pedidoDto == null)
                {
                    return NotFound(new { mensaje = $"No se encontró el pedido con ID {id}." });
                }
                return Ok(pedidoDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al obtener el detalle del pedido", error = ex.Message });
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
        public async Task<IActionResult> Put(int id, [FromBody] PedidoEdicionDTO pedidoDto)
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
        [HttpPatch("{id}/cancelar")]
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                var resultado = await _pedidoService.CancelarPedidoAsync(id);
                if (!resultado) return NotFound(new { mensaje = $"No se encontró el pedido con ID {id}." });

                return Ok(new { mensaje = "Pedido cancelado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPatch("{id}/entregar")]
        public async Task<IActionResult> Entregar(int id, [FromBody] List<int> barrilesId)
        {
            try
            {
                var pedidoDto = new PedidoEntregadoDto
                {
                    PedidoId = id,
                    BarrilesIds = barrilesId ?? new List<int>()
                };
                var resultado = await _pedidoService.EntregarPedidoAsync(pedidoDto);
                if (!resultado) return NotFound(new { mensaje = $"No se encontró el pedido con ID {id}." });

                return Ok(new { mensaje = "Pedido marcado como entregado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}