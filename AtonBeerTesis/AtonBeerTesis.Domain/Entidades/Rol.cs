using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Domain.Entidades
{
    public class Rol
    {
        public int Id { get; set; }
        [Required]
        //Esto sirve para que siempre tenga por lo menos un "" vacío nunca null y que no de errores
        public String NombreRol { get; set; } = String.Empty;
        //Coleccion para poder saber que usuarios van a tener cada rol. Un rol puede tener muchos usuarios
        public ICollection<Usuario> Usuarios { get; set; }
    }
}
