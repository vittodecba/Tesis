using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtonBeerTesis.Domain.Entities
{
    public class PlanificacionProduccion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RecetaId { get; set; }

        [ForeignKey("RecetaId")]
        public Receta? Receta { get; set; }

        [Required]
        public int FermentadorId { get; set; }

        [ForeignKey("FermentadorId")]
        // Agregamos el ? para que EF no lo exija como objeto completo al guardar
        public Fermentador? fermentador { get; set; }

        public DateTime FechaProduccion { get; set; }

        // Agregamos ? para que permitan nulos si la DB lo requiere
        public string? Estado { get; set; }

        public string? Observaciones { get; set; }

        public int UsuarioId { get; set; }

        public DateTime FechaCreacion { get; set; }

        public bool InsumosConfirmados { get; set; }
    }
}