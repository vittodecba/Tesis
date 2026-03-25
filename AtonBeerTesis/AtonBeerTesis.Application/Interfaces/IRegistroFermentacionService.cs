using AtonBeerTesis.Application.DTOs;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IRegistroFermentacionService
    {
        Task<List<RegistroFermentacionDto>> GetByLoteIdAsync(int loteId);
        Task<RegistroFermentacionDto> CreateAsync(CreateRegistroFermentacionDto dto);
        Task<bool> UpdateAsync(int id, UpdateRegistroFermentacionDto dto);
        Task<bool> DeleteAsync(int id);
    }
}