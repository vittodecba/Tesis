namespace AtonBeerTesis.Application.Dtos.BARRIL
{
    public class CreateBarrilDto
    {
        public string Codigo { get; set; } = string.Empty;
        public int FormatoEnvaseId { get; set; }
        public DateTime FechaAdquisicion { get; set; }
        public string? Observaciones { get; set; }
    }
}
