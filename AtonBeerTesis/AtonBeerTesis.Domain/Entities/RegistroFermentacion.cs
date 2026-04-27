using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtonBeerTesis.Domain.Entities
{
    public class RegistroFermentacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LoteId { get; set; }

        [ForeignKey(nameof(LoteId))]
        public Lote Lote { get; set; } = null!;

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public int DiaFermentacion { get; set; }

        [Required]
        public decimal Ph { get; set; }

        [Required]
        public decimal Densidad { get; set; } // DE

        [Required]
        public decimal Temperatura { get; set; }

        public decimal? Presion { get; set; }

        [MaxLength(200)]
        public string? Purgas { get; set; }

        [MaxLength(200)]
        public string? Extracciones { get; set; }

        [MaxLength(200)]
        public string? Agregados { get; set; }

        [MaxLength(500)]
        public string? Observaciones { get; set; }
    }
}