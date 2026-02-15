using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Domain.Entities
{
    public class MovimientoStock
    {
        public DateTime Fecha { get; set; }
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int Lote { get; set; }
        public decimal Cantidad { get; set; }
        public string TipoMovimiento { get; set; } //Refiere a "Ingreso" o "Egreso". O sea hace los calculos en el stock.
        public string MotivoMovimiento { get; set; }//Refiere a "Compra", "Venta", "Ajuste", etc. y esto hace referencia al por qué se hizo el movimiento.
        public decimal StockPrevio { get; set; }
        public decimal StockResultante { get; set; }
    }
}
