namespace AtonBeerTesis.Application.Dtos.BARRIL
{
    public class RegistrarMovimientoDto
    {
        public int BarrilId { get; set; }
        public int TipoMovimiento { get; set; }
        public int? ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
        public string? Observaciones { get; set; }
    }
}