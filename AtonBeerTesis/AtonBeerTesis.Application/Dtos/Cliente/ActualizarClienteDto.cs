using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Dtos.Cliente
{
    public class ActualizarClienteDto
    {
        public string RazonSocial { get; set; } = null!;// a averiguar
        public string Cuit { get; set; } = null!;    // a averiguar
        public string TipoCliente { get; set; } = null!;
        public string EstadoCliente { get; set; } = null!; // "Activo" | "Inactivo"

        public string? Email { get; set; }
        public string Ubicacion { get; set; } = null!;

        public string? ContactoNombre { get; set; }
        public string? ContactoTelefono { get; set; }
        public string? ContactoEmail { get; set; }
    }
}
