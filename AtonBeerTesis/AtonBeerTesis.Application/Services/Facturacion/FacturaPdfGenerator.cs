using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AtonBeerTesis.Application.Services.Facturacion
{
    // Genera el PDF de la factura con la identidad visual de AtonBeer
    // y el layout de comprobante argentino (recuadro de letra, emisor/receptor, ítems, totales).
    public static class FacturaPdfGenerator
    {
        // Paleta AtonBeer
        private const string Marron     = "#4A2C2A";
        private const string Naranja    = "#E67E22";
        private const string OrangeSoft = "#FDF1E7";
        private const string GrisBorde  = "#E5E7EB";
        private const string GrisTexto  = "#374151";

        private static readonly CultureInfo Ar = new("es-AR");
        private static string Money(decimal v) => "$ " + v.ToString("N2", Ar);

        public static byte[] Generar(FacturaPdfData d)
        {
            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(28);
                    page.DefaultTextStyle(t => t.FontSize(9).FontColor(GrisTexto));

                    page.Header().Element(c => Encabezado(c, d));
                    page.Content().Element(c => Contenido(c, d));
                    page.Footer().Element(Pie);
                });
            }).GeneratePdf();
        }

        private static void Encabezado(IContainer container, FacturaPdfData d)
        {
            container.Column(col =>
            {
                // Banda de marca con gradiente simulado (marrón → naranja en dos bloques)
                col.Item().Row(banda =>
                {
                    banda.RelativeItem().Height(6).Background(Marron);
                    banda.RelativeItem().Height(6).Background(Naranja);
                });

                col.Item().PaddingTop(10).Row(row =>
                {
                    // ── Emisor ──
                    row.RelativeItem().Column(em =>
                    {
                        em.Item().Text("AtonBeer").FontSize(20).Bold().FontColor(Marron);
                        em.Item().Text("Cerveza Artesanal").FontSize(9).FontColor(Naranja);
                        em.Item().PaddingTop(6).Text(d.EmisorRazonSocial).Bold();
                        em.Item().Text($"CUIT: {d.EmisorCuit}");
                        em.Item().Text(d.EmisorDomicilio);
                        em.Item().Text(d.EmisorCondicionIVA);
                        if (!string.IsNullOrWhiteSpace(d.EmisorIngresosBrutos))
                            em.Item().Text($"IIBB: {d.EmisorIngresosBrutos}");
                        em.Item().Text($"Inicio de actividades: {d.EmisorInicioActividades:dd/MM/yyyy}");
                    });

                    // ── Recuadro de letra ──
                    row.ConstantItem(70).Border(1).BorderColor(Marron).Column(letra =>
                    {
                        letra.Item().AlignCenter().PaddingTop(4).Text(d.Tipo)
                            .FontSize(34).Bold().FontColor(Marron);
                        letra.Item().AlignCenter().PaddingBottom(4)
                            .Text($"COD. {d.CodigoComprobante}").FontSize(8);
                    });

                    // ── Datos del comprobante ──
                    row.RelativeItem().PaddingLeft(10).Column(cp =>
                    {
                        cp.Item().AlignRight().Text("FACTURA").FontSize(18).Bold().FontColor(Marron);
                        cp.Item().AlignRight().Text($"N° {d.NumeroComprobante}").Bold();
                        cp.Item().AlignRight().Text($"Fecha: {d.Fecha:dd/MM/yyyy}");
                    });
                });

                col.Item().PaddingTop(8).LineHorizontal(1).LineColor(GrisBorde);
            });
        }

        private static void Contenido(IContainer container, FacturaPdfData d)
        {
            container.PaddingTop(10).Column(col =>
            {
                // ── Datos del cliente ──
                col.Item().Background(OrangeSoft).Border(1).BorderColor(GrisBorde).Padding(8).Column(cli =>
                {
                    cli.Item().Text(t =>
                    {
                        t.Span("Cliente: ").Bold();
                        t.Span(d.ClienteRazonSocial);
                    });
                    cli.Item().Row(r =>
                    {
                        r.RelativeItem().Text(t => { t.Span("CUIT/DNI: ").Bold(); t.Span(d.ClienteCuit); });
                        r.RelativeItem().Text(t => { t.Span("Cond. IVA: ").Bold(); t.Span(d.ClienteCondicionIVA); });
                    });
                    cli.Item().Text(t => { t.Span("Domicilio: ").Bold(); t.Span(d.ClienteDomicilio); });
                    cli.Item().Text(t => { t.Span("Condición de venta: ").Bold(); t.Span(d.CondicionVenta); });
                });

                // ── Tabla de ítems ──
                col.Item().PaddingTop(12).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(6);  // descripción
                        c.RelativeColumn(1.2f); // cantidad
                        c.RelativeColumn(2);  // precio unit
                        c.RelativeColumn(2);  // subtotal
                    });

                    table.Header(h =>
                    {
                        h.Cell().Background(Marron).Padding(5).Text("Descripción").FontColor(Colors.White).Bold();
                        h.Cell().Background(Marron).Padding(5).AlignRight().Text("Cant.").FontColor(Colors.White).Bold();
                        h.Cell().Background(Marron).Padding(5).AlignRight()
                            .Text(d.DiscriminaIva ? "P. Unit. (neto)" : "P. Unit.").FontColor(Colors.White).Bold();
                        h.Cell().Background(Marron).Padding(5).AlignRight()
                            .Text(d.DiscriminaIva ? "Subtotal (neto)" : "Subtotal").FontColor(Colors.White).Bold();
                    });

                    var zebra = false;
                    foreach (var l in d.Lineas)
                    {
                        var bg = zebra ? OrangeSoft : "#FFFFFF";
                        zebra = !zebra;

                        table.Cell().Background(bg).Padding(5).Text(l.Descripcion);
                        table.Cell().Background(bg).Padding(5).AlignRight().Text(l.Cantidad.ToString());
                        table.Cell().Background(bg).Padding(5).AlignRight().Text(Money(l.PrecioUnitario));
                        table.Cell().Background(bg).Padding(5).AlignRight().Text(Money(l.Subtotal));
                    }
                });

                // ── Totales ──
                col.Item().PaddingTop(12).AlignRight().Width(240).Column(tot =>
                {
                    if (d.DiscriminaIva)
                    {
                        TotalRow(tot, "Subtotal neto", Money(d.NetoGravado + d.Descuento));
                        if (d.Descuento > 0)
                            TotalRow(tot, "Descuento franquicia (10%)", "- " + Money(d.Descuento));
                        TotalRow(tot, "Neto gravado", Money(d.NetoGravado));
                        TotalRow(tot, "IVA 21%", Money(d.Iva));
                    }
                    else if (d.Descuento > 0)
                    {
                        TotalRow(tot, "Descuento franquicia (10%)", "- " + Money(d.Descuento));
                    }

                    tot.Item().PaddingTop(4).Background(Naranja).Padding(6).Row(r =>
                    {
                        r.RelativeItem().Text("TOTAL").FontColor(Colors.White).Bold();
                        r.RelativeItem().AlignRight().Text(Money(d.Total)).FontColor(Colors.White).Bold();
                    });
                });
            });
        }

        private static void TotalRow(ColumnDescriptor col, string label, string value)
        {
            col.Item().Row(r =>
            {
                r.RelativeItem().Text(label);
                r.RelativeItem().AlignRight().Text(value);
            });
        }

        private static void Pie(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(1).LineColor(GrisBorde);
                col.Item().PaddingTop(4).AlignCenter()
                    .Text("AtonBeer · Cerveza Artesanal")
                    .FontSize(7).FontColor(GrisTexto);
            });
        }
    }
}
