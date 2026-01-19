using AtonBeerTesis.Domain.Entidades;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtonBeerTesis.Domain.Entities
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
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
﻿using System;
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
