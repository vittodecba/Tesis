using AtonBeerTesis.Domain.Entidades;
using AtonBeerTesis.Domain.Interfaces;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class RolRepository : IRolRepository
    {
        private readonly AtonBeerDbContext _context;

        public RolRepository(AtonBeerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rol>> GetAll()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Rol> GetById(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task Add(Rol rol)
        {
            await _context.Roles.AddAsync(rol);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Rol rol)
        {
            _context.Roles.Update(rol);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol != null)
            {
                _context.Roles.Remove(rol);
                await _context.SaveChangesAsync();
            }
        }
    }
}