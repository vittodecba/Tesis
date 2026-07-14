using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
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
                .Include(l => l.Designaciones)
                    .ThenInclude(d => d.FormatoEnvase)
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
                    .ThenInclude(i => i.unidadMedida)
                    .Include(ri => ri.unidadMedida)
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

        public async Task<List<Lote>> GetFinalizadosEnRangoAsync(DateTime desde, DateTime hasta)
        {
            // Se filtra por FechaFinReal porque el reporte mide lo que se cerró en el período.
            // El límite superior se lleva al fin del día para incluir toda la fecha "hasta".
            var hastaInclusive = hasta.Date.AddDays(1).AddTicks(-1);

            return await _context.Lotes
                .Include(l => l.Receta)
                .Where(l =>
                    (l.Estado == EstadoLote.Finalizado || l.Estado == EstadoLote.Descartado) &&
                    l.FechaFinReal != null &&
                    l.FechaFinReal >= desde.Date &&
                    l.FechaFinReal <= hastaInclusive)
                .OrderByDescending(l => l.FechaFinReal)
                .ToListAsync();
        }

        public async Task<Lote?> GetActivoByFermentadorIdAsync(int fermentadorId)
        {
            // Solo retorna el lote realmente EN CURSO (EnProceso). Las reservas futuras
            // (Planificado) NO ocupan el fermentador ni se muestran acá: se ven en el listado
            // de planificación. Esto mantiene el detalle consistente con el estado del
            // fermentador (Disponible → sin lote activo; Ocupado → lote EnProceso) y evita
            // mostrar una reserva futura como "lote activo". Además, el seguimiento diario de
            // fermentación solo aplica a un lote EnProceso.
            return await _context.Lotes
                .Include(l => l.Receta)
                .Include(l => l.Fermentador)
                .Include(l => l.RegistrosFermentacion)
                .Where(l =>
                    l.FermentadorId == fermentadorId &&
                    l.Estado == EstadoLote.EnProceso)
                .OrderByDescending(l => l.Id)
                .FirstOrDefaultAsync();
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

        public async Task DeleteByIdAsync(int id)
        {
            var lote = await _context.Lotes.FindAsync(id);
            if (lote != null)
            {
                _context.Lotes.Remove(lote);
                await _context.SaveChangesAsync();
            }
        }
    }
}