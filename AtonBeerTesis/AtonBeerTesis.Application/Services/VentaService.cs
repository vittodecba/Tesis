using AtonBeerTesis.Application.Dtos.VENTAS;
using AtonBeerTesis.Application.DTOs;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AtonBeerTesis.Application.Services
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;

        public VentaService(IVentaRepository ventaRepository)
        {
            _ventaRepository = ventaRepository;
            QuestPDF.Settings.License = LicenseType.Community;
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

        public async Task<byte[]> GenerarPdfReporteVentasAsync(DateTime fechaDesde, DateTime fechaHasta)
        {
            var data = await ObtenerReporteVentasAsync(fechaDesde, fechaHasta);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("ATON BEER").FontSize(24).Black().FontColor("#4A2C2A");
                            col.Item().Text("Reporte Financiero de Ventas").FontSize(14).SemiBold().FontColor("#E67E22");
                            col.Item().Text($"Período: {fechaDesde:dd/MM/yyyy} al {fechaHasta:dd/MM/yyyy}").FontSize(10).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Spacing(20);

                        col.Item().Background(Colors.Grey.Lighten4).Padding(10).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("INGRESO TOTAL").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                                c.Item().Text($"${data.TotalVendido:N2}").FontSize(14).Black().FontColor("#3A2220");
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("CANT. VENTAS").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                                c.Item().Text($"{data.CantidadVentas}").FontSize(14).Black().FontColor("#3A2220");
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("TICKET PROMEDIO").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                                c.Item().Text($"${data.TicketPromedio:N2}").FontSize(14).Black().FontColor("#3A2220");
                            });
                        });

                        col.Item().Text("Top 5 Clientes").FontSize(14).SemiBold().FontColor("#4A2C2A");
                        col.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(120);
                            });

                            tabla.Header(header =>
                            {
                                header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Cliente").SemiBold().FontSize(10).FontColor(Colors.Grey.Darken2);
                                header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Total Comprado").SemiBold().FontSize(10).FontColor(Colors.Grey.Darken2);
                            });

                            foreach (var cliente in data.TopClientes)
                            {
                                tabla.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Text(cliente.Cliente).FontSize(10);
                                tabla.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten4).AlignRight().Text($"${cliente.TotalComprado:N2}").FontSize(10).SemiBold().FontColor("#E67E22");
                            }
                        });

                        col.Item().Text("Top Productos Vendidos").FontSize(14).SemiBold().FontColor("#4A2C2A");
                        col.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(120);
                            });

                            tabla.Header(header =>
                            {
                                header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Producto / Estilo").SemiBold().FontSize(10).FontColor(Colors.Grey.Darken2);
                                header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Unidades").SemiBold().FontSize(10).FontColor(Colors.Grey.Darken2);
                            });

                            foreach (var prod in data.TopProductos)
                            {
                                tabla.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Text(prod.Producto).FontSize(10);
                                tabla.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten4).AlignRight().Text($"{prod.CantidadVendida} u.").FontSize(10).SemiBold();
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}