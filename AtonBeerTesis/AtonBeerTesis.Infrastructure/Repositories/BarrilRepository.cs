using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
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
                .OrderBy(b => b.Codigo)
                .ToListAsync();
        }

        public async Task<Barril?> GetByIdAsync(int id)
        {
            return await _context.Barriles
                .Include(b => b.FormatoEnvase)
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

        public async Task<bool> ExisteCodigoAsync(string codigo, int? excludeId = null)
        {
            return await _context.Barriles.AnyAsync(b =>
                b.Codigo.ToLower() == codigo.ToLower() &&
                (!excludeId.HasValue || b.Id != excludeId.Value));
        }
    }
}
