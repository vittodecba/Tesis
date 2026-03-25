namespace AtonBeerTesis.Application.DTOs
{
    public class UpdateRegistroFermentacionDto
    {
        public DateTime? Fecha { get; set; }
        public int? DiaFermentacion { get; set; }
        public decimal? Ph { get; set; }
        public decimal? Densidad { get; set; }
        public decimal? Temperatura { get; set; }
        public decimal? Presion { get; set; }
        public string? Purgas { get; set; }
        public string? Extracciones { get; set; }
        public string? Agregados { get; set; }
        public string? Observaciones { get; set; }
    }
}