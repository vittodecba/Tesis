using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Dtos.Recetas;

public sealed class RecetaListItemDto
{
    public int IdReceta { get; set; }
    public string Nombre { get; set; } = null!;
    public string Estilo { get; set; } = "";
    public decimal BatchSizeLitros { get; set; }
    public RecetaEstado Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
}
