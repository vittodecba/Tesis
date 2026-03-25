using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtonBeerTesis.Domain.Entities
{
    public class LotePrueba
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Codigo { get; set; } = string.Empty;

        public int? RecetaId { get; set; }

        [ForeignKey(nameof(RecetaId))]
        public Receta? Receta { get; set; }

        [Required]
        public int FermentadorId { get; set; }

        [ForeignKey(nameof(FermentadorId))]
        public Fermentador Fermentador { get; set; } = null!;

        public int? PlanificacionProduccionId { get; set; }

        [ForeignKey(nameof(PlanificacionProduccionId))]
        public PlanificacionProduccion? PlanificacionProduccion { get; set; }

        [Required]
        public DateTime FechaElaboracion { get; set; }

        [MaxLength(100)]
        public string? Estilo { get; set; }

        [MaxLength(150)]
        public string? Inoculo { get; set; }

        [MaxLength(100)]
        public string? Responsable { get; set; }

        [Required]
        public int DiasEstimadosFermentacion { get; set; }

        [Required]
        [MaxLength(30)]
        public string Estado { get; set; } = "EnProceso";

        [MaxLength(500)]
        public string? Observaciones { get; set; }

        public DateTime? FechaFinReal { get; set; }

        public virtual ICollection<RegistroFermentacion> RegistrosFermentacion { get; set; } = new List<RegistroFermentacion>();
    }
}