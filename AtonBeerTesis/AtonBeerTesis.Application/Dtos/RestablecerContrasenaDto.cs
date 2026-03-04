using System.ComponentModel.DataAnnotations;

namespace AtonBeerTesis.Application.Dto
{
    public class RestablecerContrasenaDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código es obligatorio")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string NuevaPassword { get; set; } = string.Empty;
    }
}