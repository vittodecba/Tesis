namespace AtonBeerTesis.Application.Dtos.FACTURA
{
    public class FacturaDto
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public string NumeroVenta { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;          // "A" | "B"
        public string NumeroComprobante { get; set; } = string.Empty; // 00001-00000001
        public DateTime Fecha { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public decimal NetoGravado { get; set; }
        public decimal Descuento { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
    }
}
