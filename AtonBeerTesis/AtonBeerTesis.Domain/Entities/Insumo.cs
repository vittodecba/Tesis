using System;
using System.ComponentModel.DataAnnotations;

namespace AtonBeerTesis.Domain // OJO: Si te da error, poné el mismo namespace que dice en tu archivo "Usuario.cs"
{
    public class Insumo
    {
        [Key]
        public int Id { get; set; }

        public string NombreInsumo { get; set; } = string.Empty;

        public string Codigo { get; set; } = string.Empty; // Columna 2

        public string Tipo { get; set; } = string.Empty;   // Columna 3

        public string Unidad { get; set; } = string.Empty; // Columna 4

        public decimal StockActual { get; set; }           // Columna 5 (Cantidad numérica)

        public DateTime? UltimaActualizacion { get; set; } // Columna 6

        public string Observaciones { get; set; } = string.Empty; // Columna 7
    }
}