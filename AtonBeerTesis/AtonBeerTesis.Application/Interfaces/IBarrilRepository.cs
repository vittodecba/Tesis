using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IBarrilRepository
    {
        Task<List<Barril>> GetAllAsync();
        Task<Barril?> GetByIdAsync(int id);
        Task<Barril> AddAsync(Barril barril);
        Task<bool> UpdateAsync(Barril barril);
        Task<bool> EliminarAsync(int id);
        Task<bool> ExisteCodigoAsync(string codigo, int? excludeId = null);
        Task<List<Barril>> GetDisponiblesAsync(int formatoEnvaseId, int cantidad);
        Task MarcarComoLlenosAsync(List<int> barrilIds, int? loteId);
        Task<Dictionary<int, decimal>> ObtenerFormatosRetornablesAsync();
        Task<Barril?> ObtenerDetalleAsync(int id);
    }
}
