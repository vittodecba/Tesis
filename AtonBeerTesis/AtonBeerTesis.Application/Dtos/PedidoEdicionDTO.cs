using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Dtos
{
    public class PedidoEdicionDTO
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; }
        public List<PedidoDetalleDTO> Detalles { get; set; }
    }
}
