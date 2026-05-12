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
            var nuevoPedido = new Pedido
            {
                ClienteId = pedidoDto.IdCliente,
                Fecha = DateTime.Now,
                EstadoId = 1, // "Pendiente"
                Total = 0,
                Detalles = new List<DetallePedido>()
            };

            foreach (var item in pedidoDto.Detalles)
            {
                var detalle = new DetallePedido
                {
                    ProductoStockId = item.ProductoStockId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Precio,
                    Pedido = nuevoPedido
                };

                nuevoPedido.Total += (item.Cantidad * item.Precio);
                nuevoPedido.Detalles.Add(detalle);
            }

            var pedidoGuardado = await _pedidoRepository.AddAsync(nuevoPedido);
            return pedidoGuardado.Id;
        }
    }
}