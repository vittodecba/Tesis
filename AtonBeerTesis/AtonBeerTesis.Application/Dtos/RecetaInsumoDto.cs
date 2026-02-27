using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Dtos
{
    public class RecetaInsumoDto
    {
        public int Id { get; set; }
        public int RecetaId { get; set; }
        public int InsumoId { get; set; }
        public string? NombreInsumo { get; set; }
        public decimal Cantidad { get; set; }
        public string? UnidadMedida { get; set; }
        public int UnidadMedidaId { get; set; }
    }
}
