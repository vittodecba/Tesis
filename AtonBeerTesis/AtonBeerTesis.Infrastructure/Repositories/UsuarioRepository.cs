using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Interfaces;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context; // EL NUEVO

        public UsuarioRepository(ApplicationDbContext context) // EL NUEVO
        {
            _context = context;
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _context.usuarios
                .Include(u => u.Rol)
                .ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            // Mantenemos este que es el que usa tu Login
            return await _context.usuarios
                .Include(u => u.Rol) // Agregamos esto para que el Login sepa qué Rol tiene el usuario
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(Usuario usuario)
        {
            await _context.usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
