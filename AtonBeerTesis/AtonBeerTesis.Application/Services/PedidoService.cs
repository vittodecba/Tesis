using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoService(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<int> RegistrarPedidoAsync(PedidoCreacionDTO pedidoDto)
        {
            // 1. Creamos la cabecera del pedido
            var nuevoPedido = new Pedido
            {
                ClienteId = pedidoDto.IdCliente,
                Fecha = DateTime.Now,
                EstadoId = 1, // "Pendiente"
                Total = 0,    // Empezamos en cero para calcularlo abajo
                Detalles = new List<DetallePedido>()
            };

            // 2. Procesamos cada detalle enviado desde el DTO
            foreach (var item in pedidoDto.Detalles)
            {
                var detalle = new DetallePedido
                {
                    ProductoId = item.IdProducto,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Precio, // Usamos el precio que viene del DTO
                    Pedido = nuevoPedido
                };

                // Sumamos al total del pedido: (Cantidad * Precio)
                nuevoPedido.Total += (item.Cantidad * item.Precio);

                nuevoPedido.Detalles.Add(detalle);
            }

            // 3. Guardamos el pedido completo (Cabecera + Detalles) en la base de datos
            var pedidoGuardado = await _pedidoRepository.AddAsync(nuevoPedido);

            // 4. Devolvemos el ID generado
            return pedidoGuardado.Id;
        }
    }
}