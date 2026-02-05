using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Domain.Entities
{
    public class Insumo
    {
        public int id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;//Eso para que no sea nulo

        //Relacion con TipoInsumo
        public int TipoInsumoId { get; set; }//Clave foranea
        public TipoInsumo? TipoInsumo { get; set; }//Propiedad de navegacion, o sea lo que trae de la otra tabla
        public bool Activo { get; set; } = true;
    }
}
