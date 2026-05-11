namespace AtonBeerTesis.Application.Dtos.BARRIL
{
    public class BarrilDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public int FormatoEnvaseId { get; set; }
        public string NombreFormato { get; set; } = string.Empty;
        public decimal CapacidadLitros { get; set; }
        public int Estado { get; set; }
        public string EstadoTexto { get; set; } = string.Empty;
        public int? ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
        public DateTime FechaAdquisicion { get; set; }
        public DateTime? UltimaActualizacion { get; set; }
        public string? Observaciones { get; set; }
    }
}
