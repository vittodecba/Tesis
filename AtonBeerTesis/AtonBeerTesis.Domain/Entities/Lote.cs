using AtonBeerTesis.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtonBeerTesis.Domain.Entities
{
    public class Lote
    {
        [Key]
        public int Id { get; set; }

        // ── De HEAD (PlanificacionService) ───────────────────────────────
        [Required]
        public string CodigoLote { get; set; }

        [Required]
        public int RecetaId { get; set; }
        [ForeignKey("RecetaId")]
        public Receta Receta { get; set; }

        public int VolumenLitros { get; set; }

        public EstadoLote Estado { get; set; }

        public DateTime FechaCreacion { get; set; }

        // ── De Feature (LoteService) ──────────────────────────────────────

        // Codigo es el mismo que CodigoLote, lo exponemos como alias
        // para no romper el LoteService
        [NotMapped]
        public string Codigo
        {
            get => CodigoLote;
            set => CodigoLote = value;
        }

        [Required]
        public int FermentadorId { get; set; }
        [ForeignKey(nameof(FermentadorId))]
        public Fermentador Fermentador { get; set; } = null!;

        public DateTime FechaElaboracion { get; set; }

        [MaxLength(100)]
        public string? Estilo { get; set; }

        [MaxLength(150)]
        public string? Inoculo { get; set; }

        [MaxLength(100)]
        public string? Responsable { get; set; }

        public int DiasEstimadosFermentacion { get; set; }

        [MaxLength(500)]
        public string? Observaciones { get; set; }

        public DateTime? FechaFinReal { get; set; }

        public virtual ICollection<RegistroFermentacion> RegistrosFermentacion { get; set; }
            = new List<RegistroFermentacion>();
    }
}