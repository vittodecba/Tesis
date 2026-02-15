using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Dto
{
    public class unidadMedidaDto
    {
        public int id { get; set; }
        public string Nombre { get; set; } = string.Empty;//Aseguramos que no sea nulo
        public string Abreviatura { get; set; } = string.Empty;
    }
}
