using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Dtos.Recetas;

public sealed class UpdateRecetaDto
{
    // PATCH-like: null = no tocar
    public string? Nombre { get; set; }
    public string? Estilo { get; set; }
    public decimal? BatchSizeLitros { get; set; }
    public string? Notas { get; set; }
    public RecetaEstado? Estado { get; set; }
}
