using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Application.Dtos.Recetas;

public sealed class RecetaDetailDto
{
    public int IdReceta { get; set; }
    public string Nombre { get; set; } = null!;
    public string Estilo { get; set; } = "";
    public decimal BatchSizeLitros { get; set; }
    public string? Notas { get; set; }
    public EstadoReceta Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
