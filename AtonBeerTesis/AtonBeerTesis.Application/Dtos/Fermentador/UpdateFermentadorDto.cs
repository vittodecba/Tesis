namespace AtonBeerTesis.Application.DTOs
{
    public class UpdateFermentadorDto
    {
        public string? Nombre { get; set; }
        public int? Capacidad { get; set; }
        // Cambiamos de 'EstadoFermentador?' a 'int?'
        public int? Estado { get; set; }
        public string? Observaciones { get; set; }
    }
}