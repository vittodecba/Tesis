using AtonBeerTesis.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AtonBeerTesis.Domain.Entities
{
    public class Receta
    {
        [Key]
        public int IdReceta { get; set; }

        [Required]
        [MaxLength(120)]
        public string Nombre { get; set; } = null!;

        [MaxLength(80)]
        public string Estilo { get; set; } = "";

        public decimal BatchSizeLitros { get; set; }

        [MaxLength(1000)]
        public string? Notas { get; set; }

        public EstadoReceta Estado { get; set; } = EstadoReceta.Activa;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }
        //Relación con RecetaInsumo
        public virtual ICollection<RecetaInsumo> RecetaInsumos { get; set; } = new List<RecetaInsumo>();
        //Agregamos la relación con PasosElaboracion
        public List<PasosElaboracion> PasosElaboracion { get; set; } = new List<PasosElaboracion>();
    }
}
