using AtonBeerTesis.Application.DTOs;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface ILoteService
    {
        Task<List<LoteDto>> GetAllAsync();
        Task<LoteDetalleDto?> GetByIdAsync(int id);
        Task<LoteDto?> GetActivoByFermentadorIdAsync(int fermentadorId);
        Task<LoteDto> CreateAsync(CreateLoteDto dto);
        Task<bool> UpdateAsync(int id, UpdateLoteDto dto);
        Task<bool> FinalizarAsync(int id);
    }
}