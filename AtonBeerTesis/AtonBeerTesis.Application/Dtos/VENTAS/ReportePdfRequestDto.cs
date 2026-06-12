namespace AtonBeerTesis.Application.DTOs
{
    public class ReportePdfRequestDto
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public string TipoReporte { get; set; } = "general";
        public string? Cliente { get; set; }
        public string? GraficoPrincipalBase64 { get; set; }
        public string? GraficoSecundarioBase64 { get; set; }
    }
}