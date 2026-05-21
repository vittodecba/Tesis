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
        public string? RazonSocial { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? FechaEntregaProgramada { get; set; }
        public string? Observaciones { get; set; }
        public string? EstadoPedido { get; set; }
        public List<PedidoDetalleDTO> Detalles { get; set; }
        public decimal TotalPedido { get; set; }
    }
}
