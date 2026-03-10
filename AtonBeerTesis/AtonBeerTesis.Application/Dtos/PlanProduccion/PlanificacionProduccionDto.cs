using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Dtos
{
    public class PlanificacionProduccionDto
    {
        public int RecetaId { get; set; }
        public int FermentadorId { get; set; }
        public DateTime FechaProduccion { get; set; }
        public string? Observaciones { get; set; }
        public int UsuarioId { get; set; }
    }
}
