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

        public async Task<LotePrueba> AddAsync(LotePrueba lote)
        {
            _context.LotesPrueba.Add(lote);
            await _context.SaveChangesAsync();
            return lote;
        }

        public async Task<List<LotePrueba>> GetAllAsync()
        {
            return await _context.LotesPrueba
                .Include(l => l.Receta)
                .Include(l => l.Fermentador)
                .Include(l => l.RegistrosFermentacion)
                .OrderByDescending(l => l.Id)
                .ToListAsync();
        }

        public async Task<LotePrueba?> GetByIdAsync(int id)
        {
            return await _context.LotesPrueba
                .Include(l => l.Receta)
                .Include(l => l.Fermentador)
                .Include(l => l.RegistrosFermentacion)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<LotePrueba?> GetActivoByFermentadorIdAsync(int fermentadorId)
        {
            return await _context.LotesPrueba
                .Include(l => l.Receta)
                .Include(l => l.Fermentador)
                .FirstOrDefaultAsync(l => l.FermentadorId == fermentadorId && l.Estado == "EnProceso");
        }

        public async Task<bool> ExisteCodigoAsync(string codigo)
        {
            return await _context.LotesPrueba.AnyAsync(l => l.Codigo == codigo);
        }

        public async Task<bool> UpdateAsync(LotePrueba lote)
        {
            _context.LotesPrueba.Update(lote);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}