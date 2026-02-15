using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AtonBeerTesis.Domain.Entities; // Asegurate que este namespace coincida con tu clase TipoInsumo

namespace AtonBeerTesis.Domain
{
    public class Insumo
    {
        [Key]
        public int Id { get; set; }

        public string NombreInsumo { get; set; } = string.Empty;

        public string Codigo { get; set; } = string.Empty;

        // --- RELACIÓN CON TIPO INSUMO ---
        public int TipoInsumoId { get; set; } 
        public TipoInsumo? TipoInsumo { get; set; }
        //--- RELACION CON UNIDAD DE MEDIDA ---
      
        public int unidadMedidaId { get; set; }
        [ForeignKey("unidadMedidaId")]
        public virtual unidadMedida unidadMedida { get; set; }
        public decimal StockActual { get; set; } 

        public DateTime? UltimaActualizacion { get; set; }

        public string Observaciones { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }
}