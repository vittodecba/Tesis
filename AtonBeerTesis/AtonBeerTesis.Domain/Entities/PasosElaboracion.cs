using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AtonBeerTesis.Domain.Entities
{
    public class PasosElaboracion
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Temperatura { get; set; }
        public int Tiempo { get; set; }
        public int Orden { get; set; }
        public int RecetaId { get; set; }
        [JsonIgnore]
        public Receta? Receta { get; set; }
    }
}
