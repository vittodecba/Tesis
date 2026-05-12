using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Range(0.1, double.MaxValue, ErrorMessage = "El tiempo del paso debe ser mayor a 0")]
        public int Tiempo { get; set; }
        public int Orden { get; set; }
    }
}
