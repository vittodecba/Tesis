namespace AtonBeerTesis.Application.DTOs
{
    public class UpdateFermentadorDto
    {
        public string? Nombre { get; set; }

        [Range(1, 100000, ErrorMessage = "La capacidad debe ser mayor a 0.")]
        public int? Capacidad { get; set; }
        public int? Estado { get; set; }
        public string? Observaciones { get; set; }
    }
}