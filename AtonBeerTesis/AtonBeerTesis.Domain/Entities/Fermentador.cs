using System.ComponentModel.DataAnnotations;
using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Domain.Entities
{
    public class Fermentador
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty; // Ej: "F-01"

        [Required]
        public int Capacidad { get; set; } // Litros (Ej: 1000)

        [MaxLength(50)]

        public EstadoFermentador Estado { get; set; } = EstadoFermentador.Disponible;

        [MaxLength(500)]
        public string? Observaciones { get; set; }
    }
}