using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AtonBeerTesis.Application.Dtos.VENTAS
{
    public class AplicarDescuentoDto
    {
        public string TipoDescuento { get; set; } = string.Empty; // Porcentaje o MontoFijo
        public decimal Valor { get; set; }
        public string? Motivo { get; set; }
    }
}
