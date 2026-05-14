using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class BarrilRepository : IBarrilRepository
    {
        private readonly ApplicationDbContext _context;

        public BarrilRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Barril>> GetAllAsync()
        {
            return await _context.Barriles
                .Include(b => b.FormatoEnvase)
                .Include(b => b.Cliente)
                .OrderBy(b => b.Codigo)
                .ToListAsync();
        }

        public async Task<Barril?> GetByIdAsync(int id)
        {
            return await _context.Barriles
                .Include(b => b.FormatoEnvase)
                .Include(b => b.Cliente)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Barril> AddAsync(Barril barril)
        {
            _context.Barriles.Add(barril);
            await _context.SaveChangesAsync();
            return barril;
        }

        public async Task<bool> UpdateAsync(Barril barril)
        {
            _context.Barriles.Update(barril);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var barril = await _context.Barriles.FindAsync(id);
            if (barril == null) return false;
            _context.Barriles.Remove(barril);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Barril>> GetDisponiblesAsync(int formatoEnvaseId, int cantidad)
        {
            return await _context.Barriles
                .Where(b => b.FormatoEnvaseId == formatoEnvaseId && b.Estado == EstadoBarril.Disponible)
                .OrderBy(b => b.FechaAdquisicion)
                .Take(cantidad)
                .ToListAsync();
        }

        public async Task MarcarComoLlenosAsync(List<int> barrilIds)
        {
            if (!barrilIds.Any()) return;
            var ids = string.Join(",", barrilIds);
            await _context.Database.ExecuteSqlRawAsync(
                $"UPDATE Barriles SET Estado = 1, UltimaActualizacion = GETDATE() WHERE Id IN ({ids})");
        }

        public async Task<Dictionary<int, decimal>> ObtenerFormatosRetornablesAsync()
        {
            return await _context.Set<FormatoEnvase>()
                .AsNoTracking()
                .Where(f => f.EsRetornable)
                .ToDictionaryAsync(f => f.Id, f => f.CapacidadLitros);
        }

        public async Task<bool> ExisteCodigoAsync(string codigo, int? excludeId = null)
        {
            return await _context.Barriles.AnyAsync(b =>
                b.Codigo.ToLower() == codigo.ToLower() &&
                (!excludeId.HasValue || b.Id != excludeId.Value));
        }

        public async Task<Barril?> ObtenerDetalleAsync(int id)
        {
            return await _context.Barriles
                .Include(b => b.FormatoEnvase)
                .Include(b => b.Cliente)
                .Include(b => b.Movimientos.OrderByDescending(m => m.Fecha))
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
