namespace AtonBeerTesis.Application.Dtos.BARRIL
{
    public class UpdateBarrilDto
    {
        public int? Estado { get; set; }
        public int? ClienteId { get; set; }
        public bool DesasociarCliente { get; set; } = false;
        public DateTime? FechaAdquisicion { get; set; }
        public string? Observaciones { get; set; }
    }
}
