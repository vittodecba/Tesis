using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Dtos.Cliente
{
    internal class ClienteDto
    {
        
        public string TipoCliente { get; set; } = null!;
        public string EstadoCliente { get; set; } = null!;

        public string RazonSocial { get; set; } = null!;
        public string Cuit { get; set; } = null!;
        public string? Email { get; set; }

        public string Ubicacion { get; set; } = null!;

        public string? ContactoNombre { get; set; }
        public string? ContactoTelefono { get; set; }
        public string? ContactoEmail { get; set; }

        public DateTime? UltimaCompra { get; set; }
        public DateTime? UltimoPedido { get; set; }
        public int TotalPedidos { get; set; }
    }

}
