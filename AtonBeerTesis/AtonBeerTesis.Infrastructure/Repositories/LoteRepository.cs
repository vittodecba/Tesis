using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class LoteRepository : ILoteRepository
    {
        private readonly ApplicationDbContext _context;

        public LoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── PlanificacionService ──────────────────────────────────────────

        public async Task<Lote> CreateAsync(Lote lote)
        {
            await _context.Lotes.AddAsync(lote);
            await _context.SaveChangesAsync();
            return lote;
        }

        public async Task<Lote> GetByIdAsync(int id)
        {
            return await _context.Lotes
                .Include(l => l.Receta)
                .Include(l => l.Fermentador)
                .Include(l => l.RegistrosFermentacion)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<RecetaInsumo>> GetRecetaInsumosByLoteIdAsync(int loteId)
        {
            var lote = await _context.Lotes
                .Include(l => l.Receta)
                .FirstOrDefaultAsync(l => l.Id == loteId);

            if (lote == null) return Enumerable.Empty<RecetaInsumo>();

            return await _context.RecetaInsumos
                .Include(ri => ri.Insumo)
                .Where(ri => ri.RecetaId == lote.RecetaId)
                .ToListAsync();
        }

        // ── LoteService ───────────────────────────────────────────────────

        public async Task<List<Lote>> GetAllAsync()
        {
            return await _context.Lotes
                .Include(l => l.Receta)
                .Include(l => l.Fermentador)
                .Include(l => l.RegistrosFermentacion)
                .ToListAsync();
        }

        public async Task<Lote?> GetActivoByFermentadorIdAsync(int fermentadorId)
        {
            return await _context.Lotes
                .Include(l => l.Receta)
                .Include(l => l.Fermentador)
                .FirstOrDefaultAsync(l => l.FermentadorId == fermentadorId
                    && l.Estado != Domain.Enums.EstadoLote.Finalizado);
        }

        public async Task<bool> ExisteCodigoAsync(string codigo)
        {
            return await _context.Lotes.AnyAsync(l => l.CodigoLote == codigo);
        }

        public async Task<bool> UpdateAsync(Lote lote)
        {
            _context.Lotes.Update(lote);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}