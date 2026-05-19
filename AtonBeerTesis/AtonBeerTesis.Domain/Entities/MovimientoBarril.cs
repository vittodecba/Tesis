using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Domain.Entities
{
    public class MovimientoBarril
    {
        public int Id { get; set; }

        public int BarrilId { get; set; }
        public Barril Barril { get; set; } = null!;

        public DateTime Fecha { get; set; }

        public EstadoBarril EstadoAnterior { get; set; }

        public EstadoBarril EstadoNuevo { get; set; }

        public string Motivo { get; set; } = string.Empty;

        public string? ClienteNombre { get; set; }

        public int? LoteId { get; set; }
        public string? Observaciones { get; set; }
    }
}
