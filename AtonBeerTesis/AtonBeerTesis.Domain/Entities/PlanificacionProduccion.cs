using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Fermentador fermentador { get; set; }
        public DateTime FechaProduccion { get; set; }
        // Estados sugeridos: 1-Pendiente, 2-En Proceso, 3-Finalizado, 4-Cancelado
        public string Estado { get; set; }
        public string Observaciones { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool InsumosConfirmados { get; set; }

    }
}
