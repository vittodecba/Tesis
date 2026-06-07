using AtonBeerTesis.Application.Dtos.VENTAS;
using AtonBeerTesis.Application.DTOs;
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
            var query = _ventaRepository.GetQueryable();

            var fechaHastaReal = fechaHasta.Date.AddDays(1).AddTicks(-1);
            var diasDiferencia = (fechaHasta - fechaDesde).TotalDays;
            var fechaDesdeAnterior = fechaDesde.AddDays(-diasDiferencia - 1).Date;
            var fechaHastaAnterior = fechaDesde.AddDays(-1).Date.AddDays(1).AddTicks(-1);

            var qActuales = query.Where(v => v.FechaCreacion >= fechaDesde.Date && v.FechaCreacion <= fechaHastaReal);
            var qAnteriores = query.Where(v => v.FechaCreacion >= fechaDesdeAnterior && v.FechaCreacion <= fechaHastaAnterior);

            var totalVendido = await qActuales.SumAsync(v => v.MontoTotal);
            var cantidadVentas = await qActuales.CountAsync();
            var efectivoTotal = await qActuales.Where(v => v.MetodoPago == MetodoPago.Efectivo).SumAsync(v => v.MontoTotal);
            var transferenciaTotal = await qActuales.Where(v => v.MetodoPago == MetodoPago.Transferencia).SumAsync(v => v.MontoTotal);
            var totalAnterior = await qAnteriores.SumAsync(v => v.MontoTotal);

            var ticketPromedio = cantidadVentas > 0 ? Math.Round(totalVendido / cantidadVentas, 2) : 0;
            var variacion = totalAnterior == 0 ? 100 : Math.Round(((totalVendido - totalAnterior) / totalAnterior) * 100, 2);

            var ventasDiariasActuales = await qActuales
                .GroupBy(v => v.FechaCreacion.Day)
                .Select(g => new { Dia = g.Key, Total = g.Sum(v => v.MontoTotal) })
                .ToListAsync();

            var ventasDiariasAnteriores = await qAnteriores
                .GroupBy(v => v.FechaCreacion.Day)
                .Select(g => new { Dia = g.Key, Total = g.Sum(v => v.MontoTotal) })
                .ToListAsync();

            var comparativaMensual = ventasDiariasActuales.Select(va => new ComparativaMesDto
            {
                DiaDelPeriodo = va.Dia,
                TotalActual = va.Total,
                TotalAnterior = ventasDiariasAnteriores.FirstOrDefault(van => van.Dia == va.Dia)?.Total ?? 0
            }).OrderBy(x => x.DiaDelPeriodo).ToList();

            var topClientes = await qActuales
                .Where(v => v.Cliente != null)
                .GroupBy(v => v.Cliente.RazonSocial)
                .Select(g => new TopClienteDto
                {
                    Cliente = g.Key ?? "Desconocido",
                    TotalComprado = g.Sum(v => v.MontoTotal),
                    CantidadVentas = g.Count()
                })
                .OrderByDescending(x => x.TotalComprado)
                .Take(5)
                .ToListAsync();

            var topProductos = await qActuales
                .Where(v => v.Pedido != null)
                .SelectMany(v => v.Pedido.Detalles)
                .Where(d => d.ProductoStock != null)
                .GroupBy(d => d.ProductoStock.Estilo)
                .Select(g => new TopProductoDto
                {
                    Producto = g.Key,
                    CantidadVendida = g.Sum(d => d.Cantidad)
                })
                .OrderByDescending(x => x.CantidadVendida)
                .Take(5)
                .ToListAsync();

            var topEstilos = await qActuales
                .Where(v => v.Pedido != null)
                .SelectMany(v => v.Pedido.Detalles)
                .Where(d => d.ProductoStock != null && d.ProductoStock.Estilo != null)
                .GroupBy(d => d.ProductoStock.Estilo)
                .Select(g => new TopEstiloDto
                {
                    Estilo = g.Key,
                    CantidadVendida = g.Sum(d => d.Cantidad)
                })
                .OrderByDescending(x => x.CantidadVendida)
                .Take(5)
                .ToListAsync();

            var evolucionEstilosDb = await qActuales
                .Where(v => v.Pedido != null)
                .SelectMany(v => v.Pedido.Detalles)
                .Where(d => d.ProductoStock != null)
                .GroupBy(d => new { d.Pedido.Fecha.Date, d.ProductoStock.Estilo })
                .Select(g => new
                {
                    FechaDate = g.Key.Date,
                    Estilo = g.Key.Estilo,
                    Cantidad = g.Sum(x => x.Cantidad)
                })
                .ToListAsync();

            var evolucionLimpia = evolucionEstilosDb.Select(e => new EvolucionEstiloDto
            {
                Fecha = e.FechaDate.ToString("yyyy-MM-dd"),
                Estilo = e.Estilo,
                Cantidad = e.Cantidad
            }).OrderBy(x => x.Fecha).ToList();

            var evolucionMensualIngresos = await qActuales
                .GroupBy(v => new { v.FechaCreacion.Year, v.FechaCreacion.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(v => v.MontoTotal)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var evolucionMensualDto = evolucionMensualIngresos.Select(e => new IngresoPorMesDto
            {
                Mes = $"{e.Year}-{e.Month:D2}",
                Total = e.Total
            }).ToList();

            return new ReporteVentasDto
            {
                TotalVendido = totalVendido,
                CantidadVentas = cantidadVentas,
                EfectivoTotal = efectivoTotal,
                TransferenciaTotal = transferenciaTotal,
                TicketPromedio = ticketPromedio,
                VariacionIngresosPorcentaje = variacion,
                ComparativaMensual = comparativaMensual,
                TopClientes = topClientes,
                TopProductos = topProductos,
                TopEstilos = topEstilos,
                EvolucionEstilos = evolucionLimpia,
                EvolucionMensualIngresos = evolucionMensualDto
            };
        }
    }
}