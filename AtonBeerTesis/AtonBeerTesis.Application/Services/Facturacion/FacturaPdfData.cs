namespace AtonBeerTesis.Application.Services.Facturacion
{
    // Datos ya calculados que necesita el generador de PDF.
    public class FacturaPdfData
    {
        // Emisor
        public string EmisorRazonSocial { get; set; } = string.Empty;
        public string EmisorCuit { get; set; } = string.Empty;
        public string EmisorDomicilio { get; set; } = string.Empty;
        public string EmisorCondicionIVA { get; set; } = string.Empty;
        public string EmisorIngresosBrutos { get; set; } = string.Empty;
        public DateTime EmisorInicioActividades { get; set; }

        // Comprobante
        public string Tipo { get; set; } = "A";        // "A" | "B"
        public string CodigoComprobante { get; set; } = "01"; // 01 (A) | 06 (B)
        public string NumeroComprobante { get; set; } = string.Empty; // 00001-00000001
        public DateTime Fecha { get; set; }

        // Receptor (cliente)
        public string ClienteRazonSocial { get; set; } = string.Empty;
        public string ClienteCuit { get; set; } = string.Empty;
        public string ClienteCondicionIVA { get; set; } = string.Empty;
        public string ClienteDomicilio { get; set; } = string.Empty;
        public string CondicionVenta { get; set; } = string.Empty; // método de pago + plazo

        // Detalle
        public List<FacturaPdfLinea> Lineas { get; set; } = new();

        // Totales
        public decimal NetoGravado { get; set; }
        public decimal Descuento { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }

        public bool DiscriminaIva => Tipo == "A";
    }

    public class FacturaPdfLinea
    {
        public string Descripcion { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }   // neto unitario
        public decimal Subtotal { get; set; }         // neto (cantidad * unitario), antes de descuento
    }
}
