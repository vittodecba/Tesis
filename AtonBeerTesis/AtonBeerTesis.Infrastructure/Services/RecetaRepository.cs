using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class RecetaRepository : IRecetaRepository
    {
        private readonly AppDbContext _context;

        public RecetaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Receta>> GetAllAsync()
            => await _context.Recetas.ToListAsync();

        public async Task<Receta?> GetByIdAsync(int id)
            => await _context.Recetas.FirstOrDefaultAsync(r => r.IdReceta == id);

        public async Task AddAsync(Receta receta)
        {
            _context.Recetas.Add(receta);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Receta receta)
        {
            _context.Recetas.Update(receta);
            await _context.SaveChangesAsync();
        }
    }
}
