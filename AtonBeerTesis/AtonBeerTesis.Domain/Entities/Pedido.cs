using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtonBeerTesis.Domain.Entities
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string? Observaciones { get; set; }

        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        public int EstadoId { get; set; }
        public virtual EstadoPedido Estado { get; set; }

        public virtual ICollection<DetallePedido> Detalles { get; set; }
        public DateTime? FechaEntregaProgramada { get; set; }
    }
}