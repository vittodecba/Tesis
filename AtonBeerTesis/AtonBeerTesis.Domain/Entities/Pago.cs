using AtonBeerTesis.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Domain.Entities
{
    public class Pago
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public virtual Venta Venta { get; set; } = null!;
        public Decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public MetodoPago MetodoPago { get; set; }
    }
}
