using System.ComponentModel.DataAnnotations;

namespace AtonBeerTesis.Application.DTOs
{
    public class CreateRegistroFermentacionDto
    {
        [Required]
        public int LoteId { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public int DiaFermentacion { get; set; }

        [Required]
        public decimal Ph { get; set; }

        [Required]
        public decimal Densidad { get; set; }

        [Required]
        public decimal Temperatura { get; set; }

        public decimal? Presion { get; set; }
        public string? Purgas { get; set; }
        public string? Extracciones { get; set; }
        public string? Agregados { get; set; }
        public string? Observaciones { get; set; }
    }
}