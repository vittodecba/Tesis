using AtonBeerTesis.Application.Dtos.FACTURA;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Application.Services.Facturacion;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Application.Services
{
    public class FacturaService : IFacturaService
    {
        private const decimal AlicuotaIva = 0.21m;
        private const decimal DescuentoFranquicia = 0.10m;
        private const int PedidoFacturadoEstadoId = 3;

        private readonly IFacturaRepository _facturaRepository;
        private readonly IVentaRepository _ventaRepository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IPagoRepository _pagoRepository;

        public FacturaService(
            IFacturaRepository facturaRepository,
            IVentaRepository ventaRepository,
            IPedidoRepository pedidoRepository,
            IEmpresaRepository empresaRepository,
            IPagoRepository pagoRepository)
        {
            _facturaRepository = facturaRepository;
            _ventaRepository = ventaRepository;
            _pedidoRepository = pedidoRepository;
            _empresaRepository = empresaRepository;
            _pagoRepository = pagoRepository;
        }

        public async Task<FacturaDto> GenerarAsync(int ventaId)
        {
            var existente = await _facturaRepository.GetByVentaIdAsync(ventaId);
            if (existente is not null)
                throw new Exception("Esta venta ya tiene una factura generada.");

            var venta = await _ventaRepository.GetByIdAsync(ventaId)
                ?? throw new Exception($"No existe la venta con ID {ventaId}.");
            if (venta.EstadoVenta == EstadoVenta.Anulada)
            {
                throw new Exception("No se puede facturar una venta anulada.");
            }
            if (venta.EstadoVenta != EstadoVenta.Pagada)
                throw new Exception("No se puede facturar: la venta todavía no está 100% pagada.");

            var empresa = await _empresaRepository.GetAsync()
                ?? throw new Exception("Configure los datos de la empresa (emisor) antes de facturar.");

            var pedido = await _pedidoRepository.GetByIdAsync(venta.PedidoId)
                ?? throw new Exception($"No se encontró el pedido #{venta.PedidoId} asociado a la venta.");

            var cliente = venta.Cliente ?? pedido.Cliente
                ?? throw new Exception("La venta no tiene un cliente asociado.");


            // ── Tipo de comprobante según condición IVA del cliente ──
            var tipo = cliente.CondicionIVA is CondicionIVA.ResponsableInscripto or CondicionIVA.Monotributo
                ? TipoComprobante.A
                : TipoComprobante.B;

            // ── Cálculo tomado desde la venta ──
            var subtotal = venta.Subtotal > 0
                ? venta.Subtotal
                : pedido.Detalles.Sum(d => d.PrecioUnitario * d.Cantidad);

            var descuento = Math.Round(venta.DescuentoMonto, 2);

            var netoGravado = venta.NetoGravado > 0
                ? Math.Round(venta.NetoGravado, 2)
                : Math.Round(subtotal - descuento, 2);

            var iva = venta.IvaMonto > 0
                ? Math.Round(venta.IvaMonto, 2)
                : Math.Round(netoGravado * AlicuotaIva, 2);

            var total = venta.MontoTotal > 0
                ? Math.Round(venta.MontoTotal, 2)
                : Math.Round(netoGravado + iva, 2);

            // ── Numeración correlativa separada por tipo ──
            var ultimoNumero = await _facturaRepository.GetMaxNumeroAsync(tipo, empresa.PuntoVenta);
            var numero = ultimoNumero + 1;

            var factura = new Factura
            {
                VentaId     = venta.Id,
                Tipo        = tipo,
                PuntoVenta  = empresa.PuntoVenta,
                Numero      = numero,
                Fecha       = DateTime.Now,
                NetoGravado = netoGravado,
                Descuento   = descuento,
                Iva         = iva,
                Total       = total
            };

            // ── Generación del PDF ──
            var metodoPagoTexto = await ResolverMetodoPagoAsync(venta);
            var pdfData = ConstruirPdfData(factura, empresa, venta, pedido, cliente, metodoPagoTexto);
            var pdfBytes = FacturaPdfGenerator.Generar(pdfData);
            factura.RutaPdf = GuardarPdf(pdfBytes, factura);

            var guardada = await _facturaRepository.AddAsync(factura);

            // ── El pedido pasa a Facturado ──
            pedido.EstadoId = PedidoFacturadoEstadoId;
            await _pedidoRepository.UpdateAsync(pedido);

            return MapToDto(guardada, venta);
        }

        public async Task<FacturaDto?> ObtenerPorVentaAsync(int ventaId)
        {
            var factura = await _facturaRepository.GetByVentaIdAsync(ventaId);
            return factura is null ? null : MapToDto(factura, factura.Venta);
        }

        public async Task<(byte[] Contenido, string NombreArchivo)?> ObtenerPdfAsync(int facturaId)
        {
            var factura = await _facturaRepository.GetByIdAsync(facturaId);
            if (factura is null) return null;

            var venta = factura.Venta
                ?? throw new Exception("La factura no tiene una venta asociada.");

            var empresa = await _empresaRepository.GetAsync()
                ?? throw new Exception("No hay datos de empresa (emisor) configurados.");

            var pedido = await _pedidoRepository.GetByIdAsync(venta.PedidoId)
                ?? throw new Exception($"No se encontró el pedido #{venta.PedidoId} asociado a la venta.");

            var cliente = venta.Cliente ?? pedido.Cliente
                ?? throw new Exception("La venta no tiene un cliente asociado.");

            // Regeneramos el PDF al vuelo desde los datos guardados (no dependemos de un
            // archivo en disco, que puede no existir o tener una ruta obsoleta).
            var metodoPagoTexto = await ResolverMetodoPagoAsync(venta);
            var pdfData = ConstruirPdfData(factura, empresa, venta, pedido, cliente, metodoPagoTexto);
            var bytes = FacturaPdfGenerator.Generar(pdfData);
            var nombre = $"Factura_{factura.Tipo}_{factura.NumeroFormateado}.pdf";
            return (bytes, nombre);
        }

        // ─────────────────────────────────────────────────────────────

        // Método de cobro real según los pagos registrados:
        // un solo método, combinación de métodos separados por "/", o el método planificado si aún no hay pagos.
        private async Task<string> ResolverMetodoPagoAsync(Venta venta)
        {
            var pagos = await _pagoRepository.GetByVentaIdAsync(venta.Id);
            var metodos = pagos.Select(p => p.MetodoPago).Distinct().ToList();
            if (metodos.Count == 0) return venta.MetodoPago.ToString();
            return metodos.Count > 1
             ? string.Join(" / ", metodos.Select(m => m.ToString()))
             : metodos[0].ToString();
        }

        private static FacturaPdfData ConstruirPdfData(
            Factura factura, Empresa empresa, Venta venta, Pedido pedido, Cliente cliente,
            string metodoPagoTexto)
        {
            var descripcionDescuento = string.Empty;

            if (factura.Descuento > 0)
            {
                var motivo = string.IsNullOrWhiteSpace(venta.MotivoDescuento)
                    ? "Descuento comercial"
                    : venta.MotivoDescuento;

                var porcentaje = venta.DescuentoPorcentaje > 0
                    ? $" ({venta.DescuentoPorcentaje:0.##}%)"
                    : string.Empty;

                descripcionDescuento = $"{motivo}{porcentaje}";
            }
            var data = new FacturaPdfData
            {
                EmisorRazonSocial       = empresa.RazonSocial,
                EmisorCuit              = empresa.Cuit,
                EmisorDomicilio         = empresa.DomicilioComercial,
                EmisorCondicionIVA      = Humanizar(empresa.CondicionIVA),
                EmisorIngresosBrutos    = empresa.IngresosBrutos,
                EmisorInicioActividades = empresa.InicioActividades,

                Tipo              = factura.Tipo.ToString(),
                CodigoComprobante = factura.Tipo == TipoComprobante.A ? "01" : "06",
                NumeroComprobante = factura.NumeroFormateado,
                Fecha             = factura.Fecha,

                ClienteRazonSocial  = cliente.RazonSocial,
                ClienteCuit         = cliente.Cuit,
                ClienteCondicionIVA = Humanizar(cliente.CondicionIVA),
                ClienteDomicilio    = cliente.Ubicacion,
                CondicionVenta      = $"{metodoPagoTexto} · Vto: {venta.Plazo:dd/MM/yyyy}",

                NetoGravado = factura.NetoGravado,
                Descuento   = factura.Descuento,
                DescuentoDescripcion = descripcionDescuento,
                Iva         = factura.Iva,
                Total       = factura.Total
            };

            foreach (var d in pedido.Detalles)
            {
                var ps = d.ProductoStock;
                var descripcion =
                    $"{ps?.Estilo ?? "Sin estilo"} - {ps?.FormatoEnvase?.Nombre ?? "Sin formato"}" +
                    (ps?.FormatoEnvase != null ? $" {ps.FormatoEnvase.CapacidadLitros:0.##} L" : "");

                data.Lineas.Add(new FacturaPdfLinea
                {
                    Descripcion    = descripcion,
                    Cantidad       = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal       = d.PrecioUnitario * d.Cantidad
                });
            }

            return data;
        }

        private static string GuardarPdf(byte[] bytes, Factura factura)
        {
            var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "FacturasGeneradas");
            Directory.CreateDirectory(carpeta);

            var nombre = $"Factura_{factura.Tipo}_{factura.PuntoVenta:D5}-{factura.Numero:D8}.pdf";
            var ruta = Path.Combine(carpeta, nombre);
            File.WriteAllBytes(ruta, bytes);
            return ruta;
        }

        private static FacturaDto MapToDto(Factura f, Venta? venta) => new()
        {
            Id                = f.Id,
            VentaId           = f.VentaId,
            NumeroVenta       = venta?.NumeroVenta ?? string.Empty,
            Tipo              = f.Tipo.ToString(),
            NumeroComprobante = f.NumeroFormateado,
            Fecha             = f.Fecha,
            ClienteNombre     = venta?.Cliente?.RazonSocial ?? string.Empty,
            NetoGravado       = f.NetoGravado,
            Descuento         = f.Descuento,
            Iva               = f.Iva,
            Total             = f.Total
        };

        private static string Humanizar(CondicionIVA c) => c switch
        {
            CondicionIVA.ResponsableInscripto => "Responsable Inscripto",
            CondicionIVA.Monotributo          => "Monotributo",
            CondicionIVA.ConsumidorFinal      => "Consumidor Final",
            CondicionIVA.Exento               => "Exento",
            _                                 => c.ToString()
        };
    }
}
