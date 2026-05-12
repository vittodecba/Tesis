using System.ComponentModel.DataAnnotations;

namespace AtonBeerTesis.Application.Dto
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public int RolId { get; set; } // <--- FALTABA ESTO
        public string RolNombre { get; set; } = string.Empty;
    }

    public class UsuarioCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarPassword { get; set; } = string.Empty;

        [Required]
        public int RolId { get; set; }
    }

    public class UsuarioUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RolId { get; set; }
        public bool Activo { get; set; }
    }
}