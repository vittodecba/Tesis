using AtonBeerTesis.Domain.Enums;
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
        public int FermentadorId { get; set; }
        [ForeignKey("FermentadorId")]
        public FermentadorPrueba? FermentadorPrueba { get; set; }
        [Required]

        public int LoteId { get; set; }
        [ForeignKey("LoteId")]
        public Lote Lote { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinEstimada { get; set; }
        // Estados sugeridos: 1-Pendiente, 2-En Proceso, 3-Finalizado, 4-Cancelado
        public EstadoLote Estado { get; set; }
        public string Observaciones { get; set; }
        public int UsuarioId { get; set; }
        public bool InsumosConfirmados { get; set; }

    }
}
