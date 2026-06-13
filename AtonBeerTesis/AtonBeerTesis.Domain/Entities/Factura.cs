using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Domain.Entities
{
    // Comprobante no fiscal generado a partir de una Venta (relación 1:1).
    public class Factura
    {
        public int Id { get; set; }

        public int VentaId { get; set; }
        public virtual Venta Venta { get; set; } = null!;

        public TipoComprobante Tipo { get; set; }
        public int PuntoVenta { get; set; }
        public int Numero { get; set; }          // correlativo, secuencia separada por tipo
        public DateTime Fecha { get; set; }

        public decimal NetoGravado { get; set; }
        public decimal Descuento { get; set; }   // descuento franquicia aplicado sobre el neto
        public decimal Iva { get; set; }
        public decimal Total { get; set; }

        public string RutaPdf { get; set; } = string.Empty;

        // Número formateado AFIP: 00001-00000001
        public string NumeroFormateado => $"{PuntoVenta:D5}-{Numero:D8}";
    }
}
