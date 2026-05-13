using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Interfaces;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository, IRepository<Usuario>
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
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

        public object Add(Usuario entity)
        {
            _context.usuarios.Add(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        async Task<object> IRepository<Usuario>.AddAsync(Usuario entity)
        {
            await _context.usuarios.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public long Count(Expression<Func<Usuario, bool>> filter)
        {
            return _context.usuarios.Count(filter);
        }

        public async Task<long> CountAsync(Expression<Func<Usuario, bool>> filter)
        {
            return await _context.usuarios.CountAsync(filter);
        }

        public List<Usuario> FindAll()
        {
            return _context.usuarios.ToList();
        }

        public async Task<List<Usuario>> FindAllAsync()
        {
            return await _context.usuarios.ToListAsync();
        }

        public Usuario FindOne(params object[] keyValues)
        {
            return _context.usuarios.Find(keyValues);
        }

        public async Task<Usuario> FindOneAsync(params object[] keyValues)
        {
            return await _context.usuarios.FindAsync(keyValues);
        }

        public async Task<Usuario> FindOneAsync(object id, params string[] includeProperties)
        {
            IQueryable<Usuario> query = _context.usuarios;
            foreach (var prop in includeProperties) query = query.Include(prop);
            return await query.FirstOrDefaultAsync(u => u.Id == (int)id);
        }

        public void Remove(params object[] keyValues)
        {
            var entity = _context.usuarios.Find(keyValues);
            if (entity != null)
            {
                _context.usuarios.Remove(entity);
                _context.SaveChanges();
            }
        }

        public void Update(object id, Usuario entity)
        {
            var existing = _context.usuarios.Find(id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(entity);
                _context.SaveChanges();
            }
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync(params string[] includeProperties)
        {
            IQueryable<Usuario> query = _context.usuarios;
            foreach (var prop in includeProperties) query = query.Include(prop);
            return await query.ToListAsync();
        }
    }
}