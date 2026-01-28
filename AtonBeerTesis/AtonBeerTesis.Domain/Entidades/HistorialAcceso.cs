using AtonBeerTesis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Domain.Entidades
{
    public class HistorialAcceso
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Usuario")]//Se indica que esta propiedad es una clave foranea que hace referencia a la entidad Usuario
        public int? UsuarioId { get; set; }//Clave foranea de Usuario
        public Usuario? Usuario { get; set; }//Se crea la relacion con Usuario
        public string EmailIntentado { get; set; } = string.Empty;//Aca se guarda el email que se ingreso
        public DateTime FechaIntento { get; set; } = DateTime.Now;//El DateTime.Now guarda la fecha y hora actual del intento de acceso
        public bool Exitoso { get; set; }//True si el acceso fue exitoso, false si no lo fue
        public string Detalles { get; set; }//Detalles como por ejemplo "Contraseña incorrecta"
    }
}
