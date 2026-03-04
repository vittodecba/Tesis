using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Domain.Entities
{
    public class ProductoPrueba// Clase creada solo para hacer pruebas con Insumos, hasta tener la clase definitiva.
    {
        public int id { get; set; }
        public string Nombre { get; set; }
        public string Estilo { get; set; }
        public string Formato { get; set; }
        public string UnidadMedida { get; set; }
        public decimal StockActual { get; set; }       
    }
}
