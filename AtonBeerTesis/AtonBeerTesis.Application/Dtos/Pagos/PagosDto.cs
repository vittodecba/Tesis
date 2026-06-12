using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Dtos.Pagos
{
    public class PagosDto
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
    }
}
