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
    public class Lote
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CodigoLote { get; set; }
        [Required]
        public int RecetaId { get; set; }
        [ForeignKey("RecetaId")]
        public Receta Receta { get; set; }
        public int VolumenLitros { get; set; }
        public EstadoLote Estado { get; set; }
        public DateTime FechaCreacion { get; set; }       
    }
}
