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
        private readonly IPagoRepository _pagoRepository;
        private readonly IFacturaRepository _facturaRepository;

        public VentaService(IVentaRepository ventaRepository, IPagoRepository pagoRepository, IFacturaRepository facturaRepository)
        {
            _ventaRepository = ventaRepository;
            _pagoRepository = pagoRepository;
            _facturaRepository = facturaRepository;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<IEnumerable<VentaDto>> ObtenerTodasAsync()
        {
            var ventas = await _ventaRepository.GetAllAsync();
            var resultado = new List<VentaDto>();

            var ventasIds = ventas.Select(v => v.Id).ToList();

            var facturasPorVenta = await _facturaRepository.GetFacturaIdsPorVentaAsync();
            var pagosPorVenta = await _pagoRepository.GetTotalPagadoPorVentasAsync(ventasIds);

            var todosLosPagos = await _ventaRepository.GetQueryable()
    .Where(v => ventasIds.Contains(v.Id))
    .SelectMany(v => v.Pagos)
    .ToListAsync();

            foreach (var v in ventas)
            {
                decimal totalPagado = pagosPorVenta.TryGetValue(v.Id, out var monto) ? monto : 0m;
                var saldoPendiente = v.MontoTotal - totalPagado;

                var pagosVenta = todosLosPagos.Where(p => p.VentaId == v.Id).ToList();

                var metodosPago = pagosVenta
                    .Select(p => p.MetodoPago.ToString())
                    .Distinct()
                    .ToList();

                var metodoCobroReal = "Sin pagos";

                if (metodosPago.Count == 1)
                {
                    metodoCobroReal = metodosPago[0];
                }
                else if (metodosPago.Count > 1)
                {
                    metodoCobroReal = "Mixto";
                }

                resultado.Add(new VentaDto
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
                    MetodoPago = v.MetodoPago.ToString(),
                    MetodoCobroReal = metodoCobroReal,
                    TotalPagado = totalPagado,
                    SaldoPendiente = saldoPendiente,
                    Subtotal = v.Subtotal > 0
                    ? v.Subtotal
                    : v.NetoGravado > 0
                    ? v.NetoGravado + v.DescuentoMonto
                    : v.MontoTotal,
                    DescuentoMonto = v.DescuentoMonto,
                    DescuentoPorcentaje = v.DescuentoPorcentaje,
                    NetoGravado = v.NetoGravado,
                    IvaPorcentaje = v.IvaPorcentaje,
                    IvaMonto = v.IvaMonto,
                    MotivoDescuento = v.MotivoDescuento,
                    TieneFactura = facturasPorVenta.ContainsKey(v.Id),
                    FacturaId = facturasPorVenta.TryGetValue(v.Id, out var fid) ? fid : null
                });
            }
            return resultado;
        }

        public async Task<bool> PatchAsync(int id, PatchVentaDto dto)
        {
            var venta = await _ventaRepository.GetByIdAsync(id);
            if (venta is null) return false;

            if (venta.EstadoVenta == EstadoVenta.Pagada)
                throw new Exception("La venta ya está pagada y no puede modificarse.");

            if (dto.Plazo is not null)
            {
                if (dto.Plazo.Value.Date < DateTime.Today)
                    throw new Exception("El plazo de cobro no puede ser anterior a la fecha actual.");
                venta.Plazo = dto.Plazo.Value;
            }

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

        public async Task<bool> AplicarDescuentoAsync(int id, AplicarDescuentoDto dto)
        {
            var venta = await _ventaRepository.GetByIdAsync(id);
            if (venta is null) return false;

            if (venta.EstadoVenta == EstadoVenta.Pagada)
                throw new Exception("No se puede modificar el descuento de una venta pagada.");

            var totalPagado = await _pagoRepository.GetTotalPagadoByVentaIdAsync(id);
            if (totalPagado > 0)
                throw new Exception("No se puede modificar el descuento porque la venta ya tiene pagos registrados.");
            if (venta.EstadoVenta == EstadoVenta.Anulada)
            {
                throw new Exception("No se puede modificar el descuento de una venta anulada.");
            }
            if (dto.Valor <= 0)
                throw new Exception("El valor del descuento debe ser mayor a 0.");

            var subtotal = venta.Subtotal > 0 ? venta.Subtotal : venta.NetoGravado > 0 ? venta.NetoGravado + venta.DescuentoMonto
             : venta.MontoTotal;
            if (subtotal <= 0)
                throw new Exception("No se puede calcular el descuento porque el subtotal de la venta no es válido.");

            decimal descuentoMonto;
            decimal descuentoPorcentaje;

            var tipoDescuento = dto.TipoDescuento.Trim().ToLower();

            if (tipoDescuento == "porcentaje")
            {
                if (dto.Valor >= 100)
                    throw new Exception("El porcentaje de descuento no puede ser igual o mayor al 100%.");

                descuentoPorcentaje = dto.Valor;
                descuentoMonto = subtotal * (dto.Valor / 100);
            }
            else if (tipoDescuento == "montofijo")
            {
                descuentoMonto = dto.Valor;
                descuentoPorcentaje = (descuentoMonto / subtotal) * 100;
            }
            else
            {
                throw new Exception("Tipo de descuento inválido.");
            }

            if (descuentoMonto >= subtotal)
                throw new Exception("El descuento no puede igualar o superar el subtotal de la venta.");

            var netoGravado = subtotal - descuentoMonto;
            var ivaPorcentaje = 21m;
            var ivaMonto = Math.Round(netoGravado * (ivaPorcentaje / 100), 2);
            var totalConIva = Math.Round(netoGravado + ivaMonto, 2);

            venta.Subtotal = subtotal;
            venta.DescuentoMonto = Math.Round(descuentoMonto, 2);
            venta.DescuentoPorcentaje = Math.Round(descuentoPorcentaje, 2);
            venta.MotivoDescuento = string.IsNullOrWhiteSpace(dto.Motivo)
                ? "Descuento comercial"
                : dto.Motivo.Trim();

            venta.NetoGravado = Math.Round(netoGravado, 2);
            venta.IvaPorcentaje = ivaPorcentaje;
            venta.IvaMonto = ivaMonto;
            venta.MontoTotal = totalConIva;
            await _ventaRepository.UpdateAsync(venta);
            return true;
        }

        public async Task<ReporteVentasDto> ObtenerReporteVentasAsync(DateTime fechaDesde, DateTime fechaHasta, string? cliente = null)
        {
            var query = _ventaRepository.GetQueryable();

            if (!string.IsNullOrWhiteSpace(cliente))
            {
                query = query.Where(v => v.Cliente != null && v.Cliente.RazonSocial == cliente);
            }

            var fechaHastaReal = fechaHasta.Date.AddDays(1).AddTicks(-1);
            var diasDiferencia = (fechaHasta - fechaDesde).TotalDays;
            var fechaDesdeAnterior = fechaDesde.AddDays(-diasDiferencia - 1).Date;
            var fechaHastaAnterior = fechaDesde.AddDays(-1).Date.AddDays(1).AddTicks(-1);

            var qActuales = query.Where(v => v.FechaCreacion >= fechaDesde.Date && v.FechaCreacion <= fechaHastaReal);
            var qAnteriores = query.Where(v => v.FechaCreacion >= fechaDesdeAnterior && v.FechaCreacion <= fechaHastaAnterior);

            var totalVendido = await qActuales.SumAsync(v => v.MontoTotal);
            var cantidadVentas = await qActuales.CountAsync();
            var totalAnterior = await qAnteriores.SumAsync(v => v.MontoTotal);

            var actualesIds = await qActuales.Select(v => v.Id).ToListAsync();

            var pagosActuales = await _ventaRepository.GetQueryable()
                .Where(v => actualesIds.Contains(v.Id))
                .SelectMany(v => v.Pagos)
                .ToListAsync();

            var efectivoTotal = pagosActuales.Where(p => p.MetodoPago == MetodoPago.Efectivo).Sum(p => p.Monto);
            var transferenciaTotal = pagosActuales.Where(p => p.MetodoPago == MetodoPago.Transferencia).Sum(p => p.Monto);

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

            var comparativaMensual = Enumerable.Range(1, 31).Select(dia => new ComparativaMesDto
            {
                DiaDelPeriodo = dia,
                TotalActual = ventasDiariasActuales.FirstOrDefault(va => va.Dia == dia)?.Total ?? 0m,
                TotalAnterior = ventasDiariasAnteriores.FirstOrDefault(van => van.Dia == dia)?.Total ?? 0m
            }).ToList();

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
                .ToListAsync();

            var topProductosDb = await qActuales
                .Where(v => v.Pedido != null)
                .SelectMany(v => v.Pedido.Detalles)
                .Where(d => d.ProductoStock != null)
                .GroupBy(d => new
                {
                    Estilo = d.ProductoStock.Estilo,
                    RecetaNombre = d.ProductoStock.Receta != null ? d.ProductoStock.Receta.Nombre : "Sin Receta",
                    EnvaseNombre = d.ProductoStock.FormatoEnvase != null ? d.ProductoStock.FormatoEnvase.Nombre : "",
                    CapacidadLitros = d.ProductoStock.FormatoEnvase != null ? d.ProductoStock.FormatoEnvase.CapacidadLitros : 0
                })
                .Select(g => new
                {
                    g.Key.Estilo,
                    g.Key.RecetaNombre,
                    g.Key.EnvaseNombre,
                    g.Key.CapacidadLitros,
                    CantidadVendida = g.Sum(d => d.Cantidad)
                })
                .OrderByDescending(x => x.CantidadVendida)
                .Take(5)
                .ToListAsync();

            var topProductos = topProductosDb.Select(x => new TopProductoDto
            {
                Producto = $"Estilo: {x.Estilo} - Receta: \"{x.RecetaNombre}\" - {x.EnvaseNombre} {x.CapacidadLitros.ToString("0.##")}L",
                CantidadVendida = x.CantidadVendida
            }).ToList();

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

            var detallesEstilos = await qActuales
                .Where(v => v.Pedido != null)
                .SelectMany(v => v.Pedido.Detalles)
                .Where(d => d.ProductoStock != null)
                .Select(d => new { d.Pedido.Fecha, Estilo = d.ProductoStock.Estilo ?? "Sin Estilo", d.Cantidad })
                .ToListAsync();

            var evolucionLimpia = detallesEstilos
                .GroupBy(d => new {
                    ClaveFecha = diasDiferencia > 31 ? d.Fecha.ToString("yyyy-MM") : d.Fecha.ToString("yyyy-MM-dd"),
                    d.Estilo
                })
                .Select(g => new EvolucionEstiloDto
                {
                    Fecha = g.Key.ClaveFecha,
                    Estilo = g.Key.Estilo,
                    Cantidad = g.Sum(x => x.Cantidad)
                })
                .OrderBy(x => x.Fecha)
                .ToList();

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

        public async Task<byte[]> GenerarPdfReporteVentasAsync(ReportePdfRequestDto request)
        {
            var data = await ObtenerReporteVentasAsync(request.FechaDesde, request.FechaHasta, request.Cliente);

            byte[]? imgPrincipal = null;
            if (!string.IsNullOrEmpty(request.GraficoPrincipalBase64))
            {
                var base64Data = request.GraficoPrincipalBase64.Contains(",")
                    ? request.GraficoPrincipalBase64.Split(',')[1]
                    : request.GraficoPrincipalBase64;
                imgPrincipal = Convert.FromBase64String(base64Data);
            }

            byte[]? imgSecundario = null;
            if (!string.IsNullOrEmpty(request.GraficoSecundarioBase64))
            {
                var base64Data = request.GraficoSecundarioBase64.Contains(",")
                    ? request.GraficoSecundarioBase64.Split(',')[1]
                    : request.GraficoSecundarioBase64;
                imgSecundario = Convert.FromBase64String(base64Data);
            }

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

                            string titulo = request.TipoReporte.ToLower() switch
                            {
                                "clientes" => "Reporte de Análisis de Clientes",
                                "productos" => "Reporte de Productos y Estilos",
                                _ => "Reporte Financiero General"
                            };

                            if (!string.IsNullOrEmpty(request.Cliente))
                            {
                                titulo += $" - Cliente: {request.Cliente}";
                            }

                            col.Item().Text(titulo).FontSize(14).SemiBold().FontColor("#E67E22");
                            col.Item().Text($"Período: {request.FechaDesde:dd/MM/yyyy} al {request.FechaHasta:dd/MM/yyyy}").FontSize(10).FontColor(Colors.Grey.Medium);
                            col.Item().Text($"Fecha de Emisión: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Spacing(20);

                        if (request.TipoReporte.ToLower() == "general")
                        {
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
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().Text("VS. PERÍODO ANTERIOR").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                                    var colorVariacion = data.VariacionIngresosPorcentaje >= 0 ? "#22c55e" : "#ef4444";
                                    var signo = data.VariacionIngresosPorcentaje > 0 ? "+" : "";
                                    c.Item().Text($"{signo}{data.VariacionIngresosPorcentaje}%").FontSize(14).Black().FontColor(colorVariacion);
                                });
                            });

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Column(c => {
                                    c.Item().Text("Ingreso en Efectivo").FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                    c.Item().Text($"${data.EfectivoTotal:N2}").FontSize(12).FontColor("#22c55e");
                                });
                                row.RelativeItem().Column(c => {
                                    c.Item().Text("Ingreso por Transferencia").FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                    c.Item().Text($"${data.TransferenciaTotal:N2}").FontSize(12).FontColor("#3b82f6");
                                });
                            });
                        }
                        else if (request.TipoReporte.ToLower() == "clientes")
                        {
                            col.Item().Text(string.IsNullOrEmpty(request.Cliente) ? "Top 5 Clientes por Facturación" : "Resumen del Cliente").FontSize(14).SemiBold().FontColor("#4A2C2A");
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

                                foreach (var cliente in data.TopClientes.Take(5))
                                {
                                    tabla.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Text(cliente.Cliente).FontSize(10);
                                    tabla.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten4).AlignRight().Text($"${cliente.TotalComprado:N2}").FontSize(10).SemiBold().FontColor("#E67E22");
                                }
                            });
                        }
                        else if (request.TipoReporte.ToLower() == "productos")
                        {
                            col.Item().Text("Top 5 Productos Más Vendidos").FontSize(14).SemiBold().FontColor("#4A2C2A");
                            col.Item().Table(tabla =>
                            {
                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(120);
                                });

                                tabla.Header(header =>
                                {
                                    header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Producto").SemiBold().FontSize(10).FontColor(Colors.Grey.Darken2);
                                    header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Unidades").SemiBold().FontSize(10).FontColor(Colors.Grey.Darken2);
                                });

                                foreach (var prod in data.TopProductos)
                                {
                                    tabla.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Text(prod.Producto).FontSize(10);
                                    tabla.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten4).AlignRight().Text($"{prod.CantidadVendida} u.").FontSize(10).SemiBold();
                                }
                            });

                            col.Item().PaddingTop(15).Text("Top 5 Estilos Más Vendidos").FontSize(14).SemiBold().FontColor("#4A2C2A");
                            col.Item().Table(tabla =>
                            {
                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(120);
                                });

                                tabla.Header(header =>
                                {
                                    header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Estilo").SemiBold().FontSize(10).FontColor(Colors.Grey.Darken2);
                                    header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Unidades").SemiBold().FontSize(10).FontColor(Colors.Grey.Darken2);
                                });

                                foreach (var estilo in data.TopEstilos)
                                {
                                    tabla.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Text(estilo.Estilo).FontSize(10);
                                    tabla.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten4).AlignRight().Text($"{estilo.CantidadVendida} u.").FontSize(10).SemiBold();
                                }
                            });
                        }

                        var diasDiferencia = (request.FechaHasta - request.FechaDesde).TotalDays;
                        bool mostrarComparativa = diasDiferencia <= 31;

                        if (imgPrincipal != null)
                        {
                            string tituloPrincipal = request.TipoReporte.ToLower() switch
                            {
                                "general" => mostrarComparativa ? "Comparativa Mensual de Ingresos" : "Distribución por Pago",
                                "clientes" => "Comportamiento (Frecuencia vs Volumen)",
                                "productos" => "Evolución de Venta por Estilos",
                                _ => "Gráfico Principal"
                            };

                            col.Item().EnsureSpace().Column(c =>
                            {
                                c.Item().PaddingTop(15).Text(tituloPrincipal).FontSize(14).SemiBold().FontColor("#4A2C2A");
                                if (tituloPrincipal == "Distribución por Pago")
                                {
                                    c.Item().PaddingTop(10).AlignCenter().Box().MaxWidth(250).Image(imgPrincipal);
                                }
                                else
                                {
                                    c.Item().PaddingTop(10).Image(imgPrincipal);
                                }
                            });
                        }

                        if (imgSecundario != null)
                        {
                            string tituloSecundario = request.TipoReporte.ToLower() switch
                            {
                                "general" => mostrarComparativa ? "Distribución por Pago" : "Evolución de Ingresos (Por Mes)",
                                _ => "Gráfico Secundario"
                            };

                            col.Item().EnsureSpace().Column(c =>
                            {
                                c.Item().PaddingTop(15).Text(tituloSecundario).FontSize(14).SemiBold().FontColor("#4A2C2A");
                                if (tituloSecundario == "Distribución por Pago")
                                {
                                    c.Item().PaddingTop(10).AlignCenter().Box().MaxWidth(250).Image(imgSecundario);
                                }
                                else
                                {
                                    c.Item().PaddingTop(10).Image(imgSecundario);
                                }
                            });
                        }
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