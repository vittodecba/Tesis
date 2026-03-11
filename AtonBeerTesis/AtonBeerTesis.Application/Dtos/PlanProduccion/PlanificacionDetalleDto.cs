using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Dtos
{
    public class PlanificacionDetalleDto
    {
            public int Id { get; set; }
            public DateTime FechaProduccion { get; set; }
            public string RecetaNombre { get; set; } //Mostramos el nombre al usuario
            public string FermentadorNombre { get; set; }
            public string Estado { get; set; }
            public string? Observaciones { get; set; } 
    }
}
