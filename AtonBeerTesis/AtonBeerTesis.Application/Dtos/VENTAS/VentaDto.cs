namespace AtonBeerTesis.Application.Dtos.VENTAS
{
    public class VentaDto
    {
        public int Id { get; set; }
        public string NumeroVenta { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public int PedidoId { get; set; }
        public decimal MontoTotal { get; set; }
        public string EstadoVenta { get; set; } = string.Empty;
        public DateTime Plazo { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public decimal TotalPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string MetodoCobroReal { get; set; } = string.Empty;
        // Facturación: indica si la venta ya tiene su comprobante generado
        public bool TieneFactura { get; set; }
        public int? FacturaId { get; set; }
    }
}
