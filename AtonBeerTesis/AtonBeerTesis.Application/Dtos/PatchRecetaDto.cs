namespace AtonBeerTesis.Application.Dtos.Recetas
{
    public class PatchRecetaDto
    {
        public string? Nombre { get; set; }
        public string? Estilo { get; set; }
        public decimal? BatchSizeLitros { get; set; }
        public string? Notas { get; set; }
        public string? Estado { get; set; }
    }
}
