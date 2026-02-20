using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Dtos.Recetas;

public  class CreateRecetaDto
{
    public string Nombre { get; set; } = null!;
    public string? Estilo { get; set; }
    public decimal BatchSizeLitros { get; set; }
    public string? Notas { get; set; }
    //Agrego el insumo usado en la receta
    public List<RecetaInsumoDto> RecetaInsumos { get; set; } = new List<RecetaInsumoDto>();
}
