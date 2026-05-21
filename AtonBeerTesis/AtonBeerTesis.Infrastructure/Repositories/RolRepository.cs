using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Interfaces;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class RolRepository : IRolRepository, IRepository<Rol>
    {
        private readonly ApplicationDbContext _context; 

        public RolRepository(ApplicationDbContext context) 
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

        object IRepository<Rol>.Add(Rol entity)
        {
            _context.roles.Add(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        async Task<object> IRepository<Rol>.AddAsync(Rol entity)
        {
            await _context.roles.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public long Count(Expression<Func<Rol, bool>> filter)
        {
            return _context.roles.Count(filter);
        }

        public async Task<long> CountAsync(Expression<Func<Rol, bool>> filter)
        {
            return await _context.roles.CountAsync(filter);
        }

        public List<Rol> FindAll()
        {
            return _context.roles.ToList();
        }

        public async Task<List<Rol>> FindAllAsync()
        {
            return await _context.roles.ToListAsync();
        }

        public Rol FindOne(params object[] keyValues)
        {
            return _context.roles.Find(keyValues);
        }

        public async Task<Rol> FindOneAsync(params object[] keyValues)
        {
            return await _context.roles.FindAsync(keyValues);
        }

        public async Task<Rol> FindOneAsync(object id, params string[] includeProperties)
        {
            IQueryable<Rol> query = _context.roles;
            foreach (var prop in includeProperties) query = query.Include(prop);
            return await query.FirstOrDefaultAsync(r => r.Id == (int)id);
        }

        public void Remove(params object[] keyValues)
        {
            var entity = _context.roles.Find(keyValues);
            if (entity != null)
            {
                _context.roles.Remove(entity);
                _context.SaveChanges();
            }
        }

        public void Update(object id, Rol entity)
        {
            var existing = _context.roles.Find(id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(entity);
                _context.SaveChanges();
            }
        }

        public async Task<IEnumerable<Rol>> GetAllAsync(params string[] includeProperties)
        {
            IQueryable<Rol> query = _context.roles;
            foreach (var prop in includeProperties) query = query.Include(prop);
            return await query.ToListAsync();
        }
    }
}