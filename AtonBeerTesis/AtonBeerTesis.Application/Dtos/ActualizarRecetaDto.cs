namespace AtonBeerTesis.Application.Dtos.Recetas
{
    public class ActualizarRecetaDto
    {
        public string Nombre { get; set; } = null!;
        public string? Estilo { get; set; }
        public decimal BatchSizeLitros { get; set; }
        public string? Notas { get; set; }

        // string para que sea igual al ClienteService (Enum.TryParse)
        public string Estado { get; set; } = "Activa";
        public List<RecetaInsumoDto> RecetaInsumos { get; set; } = new List<RecetaInsumoDto>();
    }
}
