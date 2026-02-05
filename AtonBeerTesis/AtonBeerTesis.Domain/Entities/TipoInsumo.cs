using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Domain.Entities
{
    public class TipoInsumo
    {
        public int id { get; set; }
        public string Nombre { get; set; } = string.Empty;//Ejemplo: Malta, Lúpulo, Levadura, Aditivos
        public bool Activo { get; set; }=true;
    }
}
