using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Dtos.Cliente
{
    
    public class CrearClienteDto
    {
        public string RazonSocial { get; set; } = null!;
        public string Cuit { get; set; } = null!;
        public string TipoCliente { get; set; } = null!; // "Franquicia" | "Externo"

        public string? Email { get; set; }
        public string Ubicacion { get; set; } = null!;

        public string? ContactoNombre { get; set; }
        public string? ContactoTelefono { get; set; }
        public string? ContactoEmail { get; set; }
    }

}

