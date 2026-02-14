using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Interfaces;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
            return await _context.usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == email); // Quitamos AsNoTracking
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

        public object Add(Usuario entity)
        {
            throw new NotImplementedException();
        }

        Task<object> IRepository<Usuario>.AddAsync(Usuario entity)
        {
            throw new NotImplementedException();
        }

        public long Count(Expression<Func<Usuario, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync(Expression<Func<Usuario, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public List<Usuario> FindAll()
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuario>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Usuario FindOne(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario> FindOneAsync(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public void Remove(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public void Update(object id, Usuario entity)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Usuario>> IRepository<Usuario>.GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
