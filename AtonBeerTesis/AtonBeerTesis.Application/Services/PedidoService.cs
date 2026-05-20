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

        public async Task<IEnumerable<object>> ObtenerTodosAsync()
        {
            var pedidos = await _pedidoRepository.GetAllAsync();
            return pedidos.Select(p => new
            {
                idPedido = p.Id,
                clienteNombre = p.Cliente?.RazonSocial ?? "Desconocido",
                totalPedido = p.Total,
                fechaPedido = p.Fecha
            });
        }

        public async Task<int> RegistrarPedidoAsync(PedidoCreacionDTO pedidoDto)
        {
            var nuevoPedido = new Pedido
            {
                ClienteId = pedidoDto.IdCliente,
                Fecha = DateTime.Now,
                EstadoId = 1,
                Total = pedidoDto.TotalPedido,
                FechaEntregaProgramada = pedidoDto.FechaEntregaProgramada,
                Detalles = new List<DetallePedido>()
            };

            foreach (var item in pedidoDto.Detalles)
            {
                nuevoPedido.Detalles.Add(new DetallePedido
                {
                    ProductoStockId = item.ProductoStockId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Precio,
                    Pedido = nuevoPedido
                });
            }

            var pedidoGuardado = await _pedidoRepository.AddAsync(nuevoPedido);
            return pedidoGuardado.Id;
        }
    }
}