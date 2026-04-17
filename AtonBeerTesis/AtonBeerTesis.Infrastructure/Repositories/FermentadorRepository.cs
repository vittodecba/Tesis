using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class FermentadorRepository : IFermentadorRepository
    {
        private readonly ApplicationDbContext _context;

        public FermentadorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Fermentador>> GetAllConPlanificacionAsync()
        {
            return await _context.Fermentadores
                .Include(f => f.Planificaciones)
                    .ThenInclude(p => p.Lote)
                        .ThenInclude(l => l.Receta)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var fermentador = await _context.Fermentadores.FindAsync(id);
            if (fermentador == null) return false;

            // Nullear FermentadorId en lotes terminales — el histórico queda intacto
            var lotesTerminales = await _context.Lotes
                .Where(l => l.FermentadorId == id &&
                            (l.Estado == EstadoLote.Finalizado || l.Estado == EstadoLote.Descartado))
                .ToListAsync();
            foreach (var lote in lotesTerminales)
                lote.FermentadorId = null;

            // Nullear FermentadorId en planificaciones terminales
            var planTerminales = await _context.PlanificacionProduccion
                .Where(p => p.FermentadorId == id &&
                            (p.Estado == EstadoLote.Finalizado || p.Estado == EstadoLote.Descartado))
                .ToListAsync();
            foreach (var plan in planTerminales)
                plan.FermentadorId = null;

            _context.Fermentadores.Remove(fermentador);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> TieneLotesAsociadosAsync(int id)
        {
            // Solo bloquear si hay lotes activos (Planificado o EnProceso)
            return await _context.Lotes.AnyAsync(l =>
                l.FermentadorId == id &&
                l.Estado != EstadoLote.Finalizado &&
                l.Estado != EstadoLote.Descartado);
        }

        // Método 1: Traer todos
        public async Task<List<Fermentador>> GetAllAsync()
        {
            return await _context.Fermentadores.ToListAsync();
        }

        // Método 2: Buscar por ID
        public async Task<Fermentador?> GetByIdAsync(int id)
        {
            return await _context.Fermentadores.FindAsync(id);
        }

        // Método 3: Guardar nuevo
        public async Task<Fermentador> AddAsync(Fermentador fermentador)
        {
            _context.Fermentadores.Add(fermentador);
            await _context.SaveChangesAsync();
            return fermentador;
        }

        // Método 4: Modificar
        public async Task<bool> UpdateAsync(Fermentador fermentador)
        {
            _context.Fermentadores.Update(fermentador);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
