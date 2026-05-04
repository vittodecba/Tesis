using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Domain.Entities
{
    public class MovimientoStock
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int ProductoStockId { get; set; }
        public ProductoStock ProductoStock { get; set; } = null!;
        public int? LoteId { get; set; }
        public decimal Cantidad { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public string MotivoMovimiento { get; set; } = string.Empty;
        public decimal StockPrevio { get; set; }
        public decimal StockResultante { get; set; }
    }
}
