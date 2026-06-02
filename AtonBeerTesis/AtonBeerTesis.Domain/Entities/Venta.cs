using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Domain.Entities
{
    public class Venta
    {
        public int Id { get; set; }
        public string NumeroVenta { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }

        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; } = null!;

        public int PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; } = null!;

        public decimal MontoTotal { get; set; }
        public EstadoVenta EstadoVenta { get; set; } = EstadoVenta.Pendiente;
        public DateTime Plazo { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
