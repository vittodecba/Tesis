namespace AtonBeerTesis.Application.Dtos.Recetas
{
    public class RecetaDto
    {
        public int IdReceta { get; set; }
        public string Nombre { get; set; } = null!;
        public string Estilo { get; set; } = "";
        public decimal BatchSizeLitros { get; set; }
        public string? Notas { get; set; }
        public string Estado { get; set; } = "";
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        //Relacion con el RecetaInsumo
        public List<RecetaInsumoDto> RecetaInsumos { get; set; } = new List<RecetaInsumoDto>();
    }
}
