using System.ComponentModel.DataAnnotations;

namespace AtonBeerTesis.Application.DTOs
{
    public class CreateLoteDto
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public int RecetaId { get; set; }

        [Required]
        public int FermentadorId { get; set; }

        [Required]
        public DateTime FechaElaboracion { get; set; }

        public string? Estilo { get; set; }

        public string? Inoculo { get; set; }

        public string? Responsable { get; set; }

        [Required]
        public int DiasEstimadosFermentacion { get; set; }

        public string? Observaciones { get; set; }

        public int? PlanificacionProduccionId { get; set; }
    }
}