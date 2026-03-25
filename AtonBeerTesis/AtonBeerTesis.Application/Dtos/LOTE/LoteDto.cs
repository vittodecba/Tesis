namespace AtonBeerTesis.Application.DTOs
{
    public class LoteDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public int? RecetaId { get; set; }
        public string? RecetaNombre { get; set; }
        public int FermentadorId { get; set; }
        public string? FermentadorNombre { get; set; }
        public DateTime FechaElaboracion { get; set; }
        public string? Estilo { get; set; }
        public string? Inoculo { get; set; }
        public string? Responsable { get; set; }
        public int DiasEstimadosFermentacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public DateTime? FechaFinReal { get; set; }
    }
}