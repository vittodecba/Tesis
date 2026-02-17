using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Application.Dtos.Recetas;

public sealed class UpdateRecetaDto
{
    // PATCH-like: null = no tocar
    public string? Nombre { get; set; }
    public string? Estilo { get; set; }
    public decimal? BatchSizeLitros { get; set; }
    public string? Notas { get; set; }
    public EstadoReceta? Estado { get; set; }
}
