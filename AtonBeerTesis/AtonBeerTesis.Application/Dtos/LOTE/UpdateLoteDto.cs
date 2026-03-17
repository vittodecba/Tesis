namespace AtonBeerTesis.Application.DTOs
{
    public class UpdateLoteDto
    {
        public string? Codigo { get; set; }
        public int? RecetaId { get; set; }
        public DateTime? FechaElaboracion { get; set; }
        public string? Estilo { get; set; }
        public string? Inoculo { get; set; }
        public string? Responsable { get; set; }
        public int? DiasEstimadosFermentacion { get; set; }
        public string? Observaciones { get; set; }
        public string? Estado { get; set; }
    }
}