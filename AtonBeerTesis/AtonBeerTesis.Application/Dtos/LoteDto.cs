using AtonBeerTesis.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Dtos
{
    public class LoteDto
    {
        public string CodigoLote { get; set; }
        public int RecetaId { get; set; }
        public int VolumenLitros { get; set; }
        public EstadoLote Estado { get; set; }
    }
}
