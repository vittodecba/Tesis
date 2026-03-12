using System.ComponentModel.DataAnnotations;

namespace AtonBeerTesis.Application.DTOs
{
    public class CreateFermentadorDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Range(1, 100000)]
        public int Capacidad { get; set; }

        public string? Observaciones { get; set; }
    }
}