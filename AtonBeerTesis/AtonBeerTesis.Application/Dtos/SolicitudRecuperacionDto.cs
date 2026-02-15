using System.ComponentModel.DataAnnotations;

namespace AtonBeerTesis.Application.Dto
{
    public class SolicitudRecuperacionDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;
    }
}