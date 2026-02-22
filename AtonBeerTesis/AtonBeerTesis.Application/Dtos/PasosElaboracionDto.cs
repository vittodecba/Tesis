using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Dtos
{
    public class PasosElaboracionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Temperatura { get; set; }
        public int Tiempo { get; set; }
        public int Orden { get; set; }
    }
}
