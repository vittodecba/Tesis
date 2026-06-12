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

        public FacturaService(
            IFacturaRepository facturaRepository,
            IVentaRepository ventaRepository,
            IPedidoRepository pedidoRepository,
            IEmpresaRepository empresaRepository)
        {
            _facturaRepository = facturaRepository;
            _ventaRepository = ventaRepository;
            _pedidoRepository = pedidoRepository;
            _empresaRepository = empresaRepository;
        }

        public async Task<FacturaDto> GenerarAsync(int ventaId)
        {
            var existente = await _facturaRepository.GetByVentaIdAsync(ventaId);
            if (existente is not null)
                throw new Exception("Esta venta ya tiene una factura generada.");

            var venta = await _ventaRepository.GetByIdAsync(ventaId)
                ?? throw new Exception($"No existe la venta con ID {ventaId}.");

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

            // ── Cálculo (precios netos) ──
            var netoBruto = pedido.Detalles.Sum(d => d.PrecioUnitario * d.Cantidad);
            var descuento = cliente.Tipocliente == TipoCliente.Franquicia
                ? Math.Round(netoBruto * DescuentoFranquicia, 2)
                : 0m;
            var netoGravado = Math.Round(netoBruto - descuento, 2);
            var iva = Math.Round(netoGravado * AlicuotaIva, 2);
            var total = netoGravado + iva;

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
            var pdfData = ConstruirPdfData(factura, empresa, venta, pedido, cliente);
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
            var pdfData = ConstruirPdfData(factura, empresa, venta, pedido, cliente);
            var bytes = FacturaPdfGenerator.Generar(pdfData);
            var nombre = $"Factura_{factura.Tipo}_{factura.NumeroFormateado}.pdf";
            return (bytes, nombre);
        }

        // ─────────────────────────────────────────────────────────────

        private static FacturaPdfData ConstruirPdfData(
            Factura factura, Empresa empresa, Venta venta, Pedido pedido, Cliente cliente)
        {
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
                CondicionVenta      = $"{venta.MetodoPago} · Vto: {venta.Plazo:dd/MM/yyyy}",

                NetoGravado = factura.NetoGravado,
                Descuento   = factura.Descuento,
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
