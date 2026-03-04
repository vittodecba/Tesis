using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AtonBeerTesis.Domain.Entities
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Apellido { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Contrasena { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;

        // Token para recuperar contraseña (puede estar vacío si no pidió nada)
        public string? TokenRecuperacion { get; set; }

        // Fecha y hora en que vence ese token
        public DateTime? TokenExpiracion { get; set; }

        // Relación con ROL:
        // Acá le decimos que cada Usuario tiene UN Rol asignado.
        public int RolId { get; set; }

        [ForeignKey("RolId")]
        public virtual Rol? Rol { get; set; }
    }
}
