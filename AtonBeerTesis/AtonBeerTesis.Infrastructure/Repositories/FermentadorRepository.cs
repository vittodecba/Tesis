using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
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

        // Método 4: Modificar (para cuando Vitto lo use)
        public async Task<bool> UpdateAsync(Fermentador fermentador)
        {
            _context.Fermentadores.Update(fermentador);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}