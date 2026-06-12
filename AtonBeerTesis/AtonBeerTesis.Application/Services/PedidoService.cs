using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IBarrilRepository _barrilRepository;
        private readonly IVentaRepository _ventaRepository;

        public PedidoService(IPedidoRepository pedidoRepository, IBarrilRepository barrilRepository, IVentaRepository ventaRepository)
        {
            _pedidoRepository = pedidoRepository;
            _barrilRepository = barrilRepository;
            _ventaRepository = ventaRepository;
        }

        public async Task<bool> ActualizarPedidoAsync(PedidoEdicionDTO pedidoDto)
        {
            var pedidoExistente = await _pedidoRepository.GetByIdAsync(pedidoDto.Id);
            if (pedidoExistente == null)
            {
                throw new Exception($"No se encontró el pedido con ID {pedidoDto.Id}");
            }
            if (pedidoExistente.EstadoId != 1)
            {
                throw new Exception("Solo se pueden actualizar pedidos que no hayan sido entregados'");
            }

            pedidoExistente.ClienteId = pedidoDto.IdCliente;
            pedidoExistente.Observaciones = pedidoDto.Observaciones;
            pedidoExistente.FechaEntregaProgramada = pedidoDto.FechaEntregaProgramada;
            pedidoExistente.Total = pedidoDto.TotalPedido;
            var borrarPedido = pedidoExistente.Detalles.ToList();

            foreach (var detalleViejo in borrarPedido)
            {
                pedidoExistente.Detalles.Remove(detalleViejo);
            }
            foreach (var item in pedidoDto.Detalles)
            {
                var detallePedido = new DetallePedido
                {
                    ProductoStockId = item.ProductoStockId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Precio,
                    PedidoId = pedidoExistente.Id
                };
                pedidoExistente.Detalles.Add(detallePedido);
            }
            await _pedidoRepository.UpdateAsync(pedidoExistente);
            return true;
        }

        public async Task<IEnumerable<object>> ObtenerTodosAsync()
        {
            await VerificarPedidosAtrasadosAsync();
            var pedidos = await _pedidoRepository.GetAllAsync();
            return pedidos.Select(p => new
            {
                idPedido = p.Id,
                idCliente = p.ClienteId,
                clienteNombre = p.Cliente?.RazonSocial ?? "Desconocido",
                totalPedido = p.Total,
                fechaPedido = p.Fecha,
                estadoPedido = p.Estado?.Nombre ?? "Sin Estado",
                fechaEntregaProgramada = p.FechaEntregaProgramada
            });
        }
        public async Task<PedidoEdicionDTO> ObtenerPorIdAsync(int id)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(id);
            if (pedido == null) return null;
            var barrilesDelCliente = await _barrilRepository.GetAllAsync();
            var barrilesAsignadosAlCliente = barrilesDelCliente
                .Where(b => b.ClienteId == pedido.ClienteId && b.Estado == EstadoBarril.ConCliente)
                .ToList();
            return new PedidoEdicionDTO
            {
                Id = pedido.Id,
                IdCliente = pedido.ClienteId,
                RazonSocial = pedido.Cliente?.RazonSocial ?? "Empresa #" + pedido.ClienteId,
                Fecha = pedido.Fecha,
                Observaciones = pedido.Observaciones ?? "",
                EstadoPedido = pedido.Estado?.Nombre ?? "Sin Estado",
                FechaEntregaProgramada = pedido.FechaEntregaProgramada,
                TotalPedido = pedido.Total,
                Detalles = pedido.Detalles.Select(d => new PedidoDetalleDTO
                {
                    ProductoStockId = d.ProductoStockId,
                    Cantidad = d.Cantidad,
                    Precio = d.PrecioUnitario,
                    ProductoNombre =
                    $"{d.ProductoStock?.Receta?.Nombre ?? "Sin receta específica"} - " +
                    $"{d.ProductoStock?.Estilo ?? "Sin estilo"} - " +
                    $"{d.ProductoStock?.FormatoEnvase?.Nombre ?? "Sin formato"} " +
                    $"{d.ProductoStock?.FormatoEnvase?.CapacidadLitros:0.##} L",
                    BarrilesAsignados = barrilesAsignadosAlCliente
                        .Where(b => b.FormatoEnvaseId == d.ProductoStock?.FormatoEnvaseId)
                        .Select(b => b.Codigo)
                        .ToList()
                }).ToList()
            };
        }
        public async Task<int> RegistrarPedidoAsync(PedidoCreacionDTO pedidoDto)
        {
            var nuevoPedido = new Pedido
            {
                ClienteId = pedidoDto.IdCliente,
                Fecha = DateTime.Now,
                EstadoId = 1,
                Total = pedidoDto.TotalPedido,
                Observaciones = pedidoDto.Observaciones,
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
        public async Task<bool> CancelarPedidoAsync(int id)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(id);
            if (pedido == null) return false;

            if (pedido.EstadoId != 1)
            {
                throw new Exception("Solo se pueden cancelar pedidos pendientes.");
            }

            pedido.EstadoId = 4;
            await _pedidoRepository.UpdateAsync(pedido);
            return true;
        }

        public async Task<bool> EntregarPedidoAsync(PedidoEntregadoDto pedidoDto)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(pedidoDto.PedidoId);
            if (pedido == null) return false;

            if (pedido.EstadoId != 1)
                throw new Exception("Solo se pueden entregar pedidos pendientes.");

            var detallesAgrupados = pedido.Detalles
                .GroupBy(d => d.ProductoStockId)
                .Select(g => new
                {
                    ProductoStockId = g.Key,
                    Cantidad = g.Sum(x => x.Cantidad)
                })
                .ToList();

            foreach (var item in detallesAgrupados)
            {
                var productoStock = await _pedidoRepository.GetProductoStockByIdAsync(item.ProductoStockId);

                if (productoStock == null)
                    throw new Exception($"No existe el producto de stock con ID {item.ProductoStockId}.");

                if (productoStock.StockActual < item.Cantidad)
                {
                    throw new Exception(
                        $"Stock insuficiente para entregar {productoStock.Estilo}. Stock actual: {productoStock.StockActual} u. Solicitado: {item.Cantidad} u."
                    );
                }
            }

            foreach (var item in detallesAgrupados)
            {
                var productoStock = await _pedidoRepository.GetProductoStockByIdAsync(item.ProductoStockId)!;

                var stockPrevio = productoStock.StockActual;
                productoStock.StockActual -= item.Cantidad;

                await _pedidoRepository.AgregarMovimientoStockAsync(new MovimientoStock
                {
                    ProductoStockId = productoStock.Id,
                    LoteId = null,
                    Cantidad = item.Cantidad,
                    TipoMovimiento = "Egreso",
                    MotivoMovimiento = $"Entrega de pedido #{pedido.Id}",
                    StockPrevio = stockPrevio,
                    StockResultante = productoStock.StockActual,
                    Fecha = DateTime.Now
                });
            }
            if (pedidoDto.BarrilesIds != null && pedidoDto.BarrilesIds.Any())
            {
                foreach (var barrilId in pedidoDto.BarrilesIds)
                {
                    var barrilFisico = await _barrilRepository.GetByIdAsync(barrilId);

                    if (barrilFisico != null)
                    {
                        var estadoPrevio = barrilFisico.Estado;
                        barrilFisico.Estado = EstadoBarril.ConCliente;
                        barrilFisico.ClienteId = pedido.ClienteId;
                        //Aca vinculo con el movimiento automatico del barril
                        var nuevoMovimiento = new MovimientoBarril
                        {
                            Fecha = DateTime.Now,
                            EstadoAnterior = estadoPrevio,
                            EstadoNuevo = EstadoBarril.ConCliente,
                            Motivo = "Despacho a Cliente",
                            ClienteNombre = pedido.Cliente?.RazonSocial ?? $"Cliente ID: {pedido.ClienteId}",
                            Observaciones = $"Despacho automático por entrega de Pedido #{pedido.Id}"
                        };

                        barrilFisico.Movimientos.Add(nuevoMovimiento);
                        await _barrilRepository.UpdateAsync(barrilFisico);
                    }
                }
            }

            pedido.EstadoId = 2;
            await _pedidoRepository.UpdateAsync(pedido);

            var ventaExistente = await _ventaRepository.GetByPedidoIdAsync(pedido.Id);

            if (ventaExistente != null)
            {
                ventaExistente.ClienteId = pedido.ClienteId;
                ventaExistente.MontoTotal = pedido.Total;
                ventaExistente.EstadoVenta = EstadoVenta.Pendiente;
                ventaExistente.Plazo = pedidoDto.Plazo;
                ventaExistente.MetodoPago = pedidoDto.MetodoPago;

                await _ventaRepository.UpdateAsync(ventaExistente);
            }
            else
            {
                var nuevaVenta = new Venta
                {
                    FechaCreacion = DateTime.Now,
                    ClienteId = pedido.ClienteId,
                    PedidoId = pedido.Id,
                    MontoTotal = pedido.Total,
                    EstadoVenta = EstadoVenta.Pendiente,
                    Plazo = pedidoDto.Plazo,
                    MetodoPago = pedidoDto.MetodoPago,
                    NumeroVenta = string.Empty
                };

                var ventaGuardada = await _ventaRepository.AddAsync(nuevaVenta);
                ventaGuardada.NumeroVenta = $"VNT-{DateTime.Now.Year}-{ventaGuardada.Id:D5}";
                await _ventaRepository.UpdateAsync(ventaGuardada);
            }

            return true;
        }
        public async Task<bool> DeshacerEntregaPedidoAsync(int pedidoId)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
            if (pedido == null) return false;
            if (pedido.EstadoId == 3)
            {
                throw new Exception("El pedido ya fue facturado; no se puede deshacer la entrega.");
            }
            if (pedido.EstadoId != 2)
            {
                throw new Exception("Solo se puede deshacer la entrega de pedidos que figuren como 'Entregados'.");
            }

            var detallesAgrupados = pedido.Detalles
                .GroupBy(d => d.ProductoStockId)
                .Select(g => new
                {
                    ProductoStockId = g.Key,
                    Cantidad = g.Sum(x => x.Cantidad)
                })
                .ToList();

            foreach (var item in detallesAgrupados)
            {
                var productoStock = await _pedidoRepository.GetProductoStockByIdAsync(item.ProductoStockId);

                if (productoStock != null)
                {
                    var stockPrevio = productoStock.StockActual;
                    productoStock.StockActual += item.Cantidad;

                    await _pedidoRepository.AgregarMovimientoStockAsync(new MovimientoStock
                    {
                        ProductoStockId = productoStock.Id,
                        LoteId = null,
                        Cantidad = item.Cantidad,
                        TipoMovimiento = "Ingreso",
                        MotivoMovimiento = $"Reversión por cancelación de entrega de Pedido #{pedido.Id}",
                        StockPrevio = stockPrevio,
                        StockResultante = productoStock.StockActual,
                        Fecha = DateTime.Now
                    });
                }
            }
            var todosLosBarriles = await _barrilRepository.GetAllAsync();
            var barrilesDelPedido = todosLosBarriles
                .Where(b => b.ClienteId == pedido.ClienteId && b.Estado == EstadoBarril.ConCliente)
                .Where(b => b.Movimientos != null && b.Movimientos.Any(m => m.Observaciones != null && m.Observaciones.Contains($"Pedido #{pedido.Id}")))
                .ToList();

            foreach (var barrilFisico in barrilesDelPedido)
            {
                var estadoPrevio = barrilFisico.Estado;

                barrilFisico.Estado = EstadoBarril.Lleno;
                barrilFisico.ClienteId = null;
                var movimientoReversion = new MovimientoBarril
                {
                    Fecha = DateTime.Now,
                    EstadoAnterior = estadoPrevio,
                    EstadoNuevo = EstadoBarril.Lleno,
                    Motivo = "ReversiónEntrega",
                    ClienteNombre = pedido.Cliente?.RazonSocial ?? $"Cliente ID: {pedido.ClienteId}",
                    Observaciones = $"Reversión automática por cancelación de entrega de Pedido #{pedido.Id}",
                    LoteId = barrilFisico.LoteActualId
                };

                barrilFisico.Movimientos.Add(movimientoReversion);
                await _barrilRepository.UpdateAsync(barrilFisico);
            }
            pedido.EstadoId = 1;
            await _pedidoRepository.UpdateAsync(pedido);

            return true;
        }

        private async Task VerificarPedidosAtrasadosAsync()
        {
            var hoy = DateTime.Today;

            var pedidosVencidos = await _pedidoRepository.GetPedidosVencidosAsync(hoy, 1);

            if (pedidosVencidos != null && pedidosVencidos.Any())
            {               
                foreach (var p in pedidosVencidos)
                {
                    p.EstadoId = 5;
                    await _pedidoRepository.UpdateAsync(p);
                }
            }
        }
    }
}