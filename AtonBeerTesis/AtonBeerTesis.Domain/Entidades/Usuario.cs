using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Domain.Entidades
{
    public class Usuario
    {
        public int id { get; set; }
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Apellido { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;//Aca va la contrasena a hashear despues

        public bool Activo { get; set; } = true;

        //Clave foranea para relacionarla con el rol
        public int RolId { get; set; }

        //Propiedad de navegacion para ver en este caso saber que rol tiene cada usuario. Un usuario puede tener un rol
        public  Rol Rol { get; set; }

    }
}
