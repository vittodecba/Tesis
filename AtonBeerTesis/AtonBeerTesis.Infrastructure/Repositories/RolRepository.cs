using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Interfaces;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class RolRepository : IRolRepository
    {
        private readonly ApplicationDbContext _context; // EL NUEVO

        public RolRepository(ApplicationDbContext context) // EL NUEVO
        {
            _context = context;
        }

        public async Task<IEnumerable<Rol>> GetAll()
        {
            return await _context.roles.ToListAsync();
        }

        public async Task<Rol> GetById(int id)
        {
            return await _context.roles.FindAsync(id);
        }

        public async Task Add(Rol rol)
        {
            await _context.roles.AddAsync(rol);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Rol rol)
        {
            _context.roles.Update(rol);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var rol = await _context.roles.FindAsync(id);
            if (rol != null)
            {
                _context.roles.Remove(rol);
                await _context.SaveChangesAsync();
            }
        }
    }
}