using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Domain.Entities
{
    public class PedidoEntregadoDto
    {
        public int PedidoId { get; set; }        
        public List<int> BarrilesIds { get; set; } = new List<int>();
    }
}
