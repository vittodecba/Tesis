using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Dtos.Recetas;
using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IRecetaService
    {
        Task<List<RecetaDto>> GetAllAsync(string? nombre = null, string? estilo = null, string? estado = null, string? orden = null);
        Task<RecetaDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateRecetaDto dto);
        Task<bool> UpdateAsync(int id, ActualizarRecetaDto dto);
        Task<bool> PatchAsync(int id, PatchRecetaDto dto);
        Task<bool> DeactivateAsync(int id);
        List<string> GetEstadosReceta();
        Task<bool> AddInsumoToReceta(int id, RecetaInsumoDto dto);
        Task<bool> RemoveInsumoDeReceta(int id, int insumoId);
    }

    public interface IRecetaRepository
    {
        Task<List<Receta>> GetAllAsync(string? nombre = null, string? estilo = null, string? estado = null, string? orden = null);
        Task<Receta?> GetByIdAsync(int id);
        Task AddAsync(Receta receta);
        Task UpdateAsync(Receta receta);
        Task<bool> AddInsumoAsync(RecetaInsumo relacion);
        Task<bool> RemoveInsumoAsync(int idReceta, int idInsumo);
    }
}