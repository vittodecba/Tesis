using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IRegistroFermentacionRepository
    {
        Task<RegistroFermentacion> AddAsync(RegistroFermentacion registro);
        Task<RegistroFermentacion?> GetByIdAsync(int id);
        Task<List<RegistroFermentacion>> GetByLoteIdAsync(int loteId);
        Task<bool> ExistePorFechaAsync(int loteId, DateTime fecha);
        Task<bool> ExistePorDiaAsync(int loteId, int diaFermentacion);
        Task<bool> UpdateAsync(RegistroFermentacion registro);
        Task<bool> DeleteAsync(RegistroFermentacion registro);
    }
}