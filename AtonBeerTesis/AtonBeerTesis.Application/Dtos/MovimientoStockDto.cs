using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Dto
{
    public class MovimientoStockDto
    {      
        public int productoId { get; set; }
        public decimal cantidad { get; set; }
        public string? tipoMovimiento { get; set; } // "Ingreso" o "Egreso"
        public string? motivoMovimiento { get; set; }
        public DateTime Fecha { get; set; }
    }
}
