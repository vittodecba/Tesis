using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface ILoteRepository
    {
        // Métodos del PlanificacionService (HEAD)
        Task<Lote> CreateAsync(Lote lote);
        Task<Lote> GetByIdAsync(int id);
        Task<IEnumerable<RecetaInsumo>> GetRecetaInsumosByLoteIdAsync(int loteId);

        // Métodos del LoteService (Feature)
        Task<List<Lote>> GetAllAsync();

        // Reporte P2 · Cumplimiento: lotes cerrados (Finalizado/Descartado) cuya
        // FechaFinReal cae dentro del rango, con la receta cargada para el detalle.
        Task<List<Lote>> GetFinalizadosEnRangoAsync(DateTime desde, DateTime hasta);

        Task<Lote?> GetActivoByFermentadorIdAsync(int fermentadorId);
        Task<bool> ExisteCodigoAsync(string codigo);
        Task<bool> UpdateAsync(Lote lote);
        Task DeleteByIdAsync(int id);
    }
}