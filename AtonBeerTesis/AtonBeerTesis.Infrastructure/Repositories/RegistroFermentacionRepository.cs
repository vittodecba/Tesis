using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class RegistroFermentacionRepository : IRegistroFermentacionRepository
    {
        private readonly ApplicationDbContext _context;

        public RegistroFermentacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RegistroFermentacion> AddAsync(RegistroFermentacion registro)
        {
            _context.RegistrosFermentacion.Add(registro);
            await _context.SaveChangesAsync();
            return registro;
        }

        public async Task<RegistroFermentacion?> GetByIdAsync(int id)
        {
            return await _context.RegistrosFermentacion
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<RegistroFermentacion>> GetByLoteIdAsync(int loteId)
        {
            return await _context.RegistrosFermentacion
                .Where(r => r.LoteId == loteId)
                .OrderBy(r => r.DiaFermentacion)
                .ToListAsync();
        }

        public async Task<bool> ExistePorFechaAsync(int loteId, DateTime fecha)
        {
            return await _context.RegistrosFermentacion
                .AnyAsync(r => r.LoteId == loteId && r.Fecha.Date == fecha.Date);
        }

        public async Task<bool> ExistePorDiaAsync(int loteId, int diaFermentacion)
        {
            return await _context.RegistrosFermentacion
                .AnyAsync(r => r.LoteId == loteId && r.DiaFermentacion == diaFermentacion);
        }

        public async Task<bool> UpdateAsync(RegistroFermentacion registro)
        {
            _context.RegistrosFermentacion.Update(registro);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(RegistroFermentacion registro)
        {
            _context.RegistrosFermentacion.Remove(registro);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}