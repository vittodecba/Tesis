using AtonBeerBackend.Models.DTOs;
using AtonBeerTesis.Application.Dtos.VENTAS;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Application.Services
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;

        public VentaService(IVentaRepository ventaRepository)
        {
            _ventaRepository = ventaRepository;
        }

        public async Task<IEnumerable<VentaDto>> ObtenerTodasAsync()
        {
            var ventas = await _ventaRepository.GetAllAsync();
            return ventas.Select(v => new VentaDto
            {
                Id = v.Id,
                NumeroVenta = v.NumeroVenta,
                FechaCreacion = v.FechaCreacion,
                ClienteId = v.ClienteId,
                ClienteNombre = v.Cliente?.RazonSocial ?? $"Cliente #{v.ClienteId}",
                PedidoId = v.PedidoId,
                MontoTotal = v.MontoTotal,
                EstadoVenta = v.EstadoVenta.ToString(),
                Plazo = v.Plazo,
                MetodoPago = v.MetodoPago.ToString()
            });
        }

        public async Task<bool> PatchAsync(int id, PatchVentaDto dto)
        {
            var venta = await _ventaRepository.GetByIdAsync(id);
            if (venta is null) return false;

            // BLOQUEO: venta pagada no puede modificarse
            if (venta.EstadoVenta == EstadoVenta.Pagado)
                throw new Exception("La venta ya está pagada y no puede modificarse.");

            if (dto.Plazo is not null)
                venta.Plazo = dto.Plazo.Value;

            if (dto.MetodoPago is not null)
            {
                if (!Enum.TryParse<MetodoPago>(dto.MetodoPago, ignoreCase: true, out var metodo))
                    throw new Exception($"Método de pago inválido: '{dto.MetodoPago}'.");
                venta.MetodoPago = metodo;
            }

            if (dto.EstadoVenta is not null)
            {
                if (!Enum.TryParse<EstadoVenta>(dto.EstadoVenta, ignoreCase: true, out var estado))
                    throw new Exception($"Estado de venta inválido: '{dto.EstadoVenta}'.");
                venta.EstadoVenta = estado;
            }

            await _ventaRepository.UpdateAsync(venta);
            return true;
        }

        public async Task<ReporteVentasDto> ObtenerReporteVentasAsync(DateTime fechaDesde, DateTime fechaHasta)
        {
            var fechaHastaFin = fechaHasta.Date.AddDays(1).AddTicks(-1);
            var todasLasVentas = await _ventaRepository.GetAllAsync();

            var ventasActuales = todasLasVentas
                .Where(v => v.FechaCreacion >= fechaDesde.Date && v.FechaCreacion <= fechaHastaFin)
                .ToList();

            var diasPeriodo = (fechaHasta.Date - fechaDesde.Date).Days + 1;
            var fechaDesdeAnterior = fechaDesde.Date.AddDays(-diasPeriodo);
            var fechaHastaAnteriorFin = fechaDesde.Date.AddTicks(-1);

            var ventasAnteriores = todasLasVentas
                .Where(v => v.FechaCreacion >= fechaDesdeAnterior && v.FechaCreacion <= fechaHastaAnteriorFin)
                .ToList();

            var totalVendidoActual = ventasActuales.Sum(v => v.MontoTotal);
            var totalVendidoAnterior = ventasAnteriores.Sum(v => v.MontoTotal);

            decimal variacion = 0;
            if (totalVendidoAnterior > 0)
            {
                variacion = ((totalVendidoActual - totalVendidoAnterior) / totalVendidoAnterior) * 100;
            }
            else if (totalVendidoActual > 0)
            {
                variacion = 100;
            }

            var reporte = new ReporteVentasDto
            {
                TotalVendido = totalVendidoActual,
                CantidadVentas = ventasActuales.Count,
                EfectivoTotal = ventasActuales.Where(v => v.MetodoPago == MetodoPago.Efectivo).Sum(v => v.MontoTotal),
                TransferenciaTotal = ventasActuales.Where(v => v.MetodoPago == MetodoPago.Transferencia).Sum(v => v.MontoTotal),
                TicketPromedio = ventasActuales.Count > 0 ? totalVendidoActual / ventasActuales.Count : 0,
                VariacionIngresosPorcentaje = Math.Round(variacion, 2),
                VentasPorDia = ventasActuales
                    .GroupBy(v => v.FechaCreacion.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new VentaPorDiaDto
                    {
                        Fecha = g.Key.ToString("yyyy-MM-dd"),
                        Total = g.Sum(v => v.MontoTotal)
                    })
                    .ToList(),
                TopClientes = ventasActuales
                    .Where(v => v.Cliente != null)
                    .GroupBy(v => v.Cliente.RazonSocial)
                    .Select(g => new TopClienteDto
                    {
                        Cliente = string.IsNullOrEmpty(g.Key) ? "Consumidor Final" : g.Key,
                        TotalComprado = g.Sum(v => v.MontoTotal),
                        CantidadVentas = g.Count()
                    })
                    .OrderByDescending(x => x.TotalComprado)
                    .Take(5)
                    .ToList()
            };

            var detallesActuales = ventasActuales
                .Where(v => v.Pedido != null && v.Pedido.Detalles != null)
                .SelectMany(v => v.Pedido.Detalles)
                .ToList();

            reporte.TopProductos = detallesActuales
                .Where(d => d.ProductoStock != null)
                .GroupBy(d => new
                {
                    EnvaseNombre = d.ProductoStock.FormatoEnvase != null ? d.ProductoStock.FormatoEnvase.Nombre : "Sin envase",
                    EnvaseLitros = d.ProductoStock.FormatoEnvase != null ? d.ProductoStock.FormatoEnvase.CapacidadLitros : 0,
                    Estilo = d.ProductoStock.Estilo,
                    RecetaNombre = d.ProductoStock.Receta != null ? d.ProductoStock.Receta.Nombre : "Sin receta"
                })
                .Select(g => new TopProductoDto
                {
                    Producto = $"{g.Key.EnvaseNombre} {g.Key.EnvaseLitros.ToString("0.##")}L - Estilo: {g.Key.Estilo.ToUpper()} - Receta: \"{g.Key.RecetaNombre}\"",
                    CantidadVendida = g.Sum(d => d.Cantidad)
                })
                .OrderByDescending(x => x.CantidadVendida)
                .Take(5)
                .ToList();

            reporte.TopEstilos = detallesActuales
                .Where(d => d.ProductoStock != null && !string.IsNullOrEmpty(d.ProductoStock.Estilo))
                .GroupBy(d => d.ProductoStock.Estilo)
                .Select(g => new TopEstiloDto
                {
                    Estilo = g.Key,
                    CantidadVendida = g.Sum(d => d.Cantidad)
                })
                .OrderByDescending(x => x.CantidadVendida)
                .Take(5)
                .ToList();

            return reporte;
        }
    }
}