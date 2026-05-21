using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Domain.Entities
{
    public class Barril
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public int FormatoEnvaseId { get; set; }
        public FormatoEnvase FormatoEnvase { get; set; } = null!;

        public EstadoBarril Estado { get; set; } = EstadoBarril.Disponible;

        public int? ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public int? LoteActualId { get; set; }
        public Lote? LoteActual { get; set; }

        public DateTime FechaAdquisicion { get; set; }

        public DateTime? UltimaActualizacion { get; set; }

        public string? Observaciones { get; set; }

        public ICollection<MovimientoBarril> Movimientos { get; set; } = new List<MovimientoBarril>();
    }
}