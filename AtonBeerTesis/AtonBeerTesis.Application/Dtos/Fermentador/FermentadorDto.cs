namespace AtonBeerTesis.Application.DTOs
{
    public class FermentadorDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public string Estado { get; set; } = string.Empty; // Lo mandamos como texto ("Disponible")
        public string? Observaciones { get; set; }

        public int? LoteId { get; set; }
        public string? EstiloNombre { get; set; }
    }
}