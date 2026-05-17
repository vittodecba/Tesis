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

        public async Task<bool> ActualizarPedidoAsync(PedidoEdicionDTO pedidoDto)
        {
            var pedidoExistente = await _pedidoRepository.GetByIdAsync(pedidoDto.Id);
            if (pedidoExistente == null)
            {
                throw new Exception($"No se encontró el pedido con ID {pedidoDto.Id}");
            }

            //Validacion de que solo se puedan actualizar pedidos que ya han sido entregados
            //imaginando que el 1 = pendiente y el 2 = entregado
            if (pedidoExistente.EstadoId != 1)
            {
                throw new Exception("Solo se pueden actualizar pedidos que no hayan sido entregados'");
            }

            pedidoExistente.ClienteId = pedidoDto.IdCliente;
            pedidoExistente.Fecha = pedidoDto.Fecha;
            pedidoExistente.Detalles.Clear();
            decimal nuevoTotal = 0;
            foreach(var item in pedidoDto.Detalles)
            {
                var detallePedido = new DetallePedido
                {
                    ProductoStockId = item.ProductoStockId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Precio,
                    PedidoId = pedidoExistente.Id
                };
                nuevoTotal += (item.Cantidad * item.Precio);
                pedidoExistente.Detalles.Add(detallePedido);
            }
            pedidoExistente.Total = nuevoTotal;
            await _pedidoRepository.UpdateAsync(pedidoExistente);
            return true;
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