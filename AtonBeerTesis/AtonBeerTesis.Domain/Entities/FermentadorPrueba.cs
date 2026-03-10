using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Domain.Entities
{
    public class FermentadorPrueba
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String Nombre { get; set; } = string.Empty;//Ej: "Fermentador 1"
        public bool Disponibilidad { get; set; } = true;//Arranca disponible
        public decimal Capacidad { get; set; }
    }
}
