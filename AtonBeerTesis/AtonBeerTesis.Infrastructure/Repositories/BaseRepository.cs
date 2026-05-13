using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using AtonBeerTesis.Domain.Interfaces;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public object Add(TEntity entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();

            try
            {
                return _context.Entry(entity).Property("Id").CurrentValue;
            }
            catch
            {
                return null;
            }
        }

        public async Task<object> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            try
            {
                return _context.Entry(entity).Property("Id").CurrentValue;
            }
            catch
            {
                return null;
            }
        }

        public long Count(Expression<Func<TEntity, bool>> filter)
        {
            return _dbSet.Count(filter);
        }

        public async Task<long> CountAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _dbSet.CountAsync(filter);
        }

        public List<TEntity> FindAll()
        {
            return _dbSet.ToList();
        }

        public async Task<List<TEntity>> FindAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public TEntity FindOne(params object[] keyValues)
        {
            return _dbSet.Find(keyValues);
        }

        public async Task<TEntity> FindOneAsync(params object[] keyValues)
        {
            return await _dbSet.FindAsync(keyValues);
        }

        public async Task<TEntity> FindOneAsync(object id, params string[] includeProperties)
        {
            IQueryable<TEntity> query = _dbSet;
            foreach (var property in includeProperties)
            {
                query = query.Include(property);
            }
            return await query.FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id));
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(params string[] includeProperties)
        {
            IQueryable<TEntity> query = _dbSet;
            foreach (var property in includeProperties)
            {
                query = query.Include(property);
            }
            return await query.ToListAsync();
        }

        public void Remove(params object[] keyValues)
        {
            var entity = FindOne(keyValues);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                _context.SaveChanges();
            }
        }

        public void Update(object id, TEntity entity)
        {
            var foundEntity = FindOne(id);
            if (foundEntity != null)
            {
                _context.Entry(foundEntity).CurrentValues.SetValues(entity);
                _context.SaveChanges();
            }
        }
    }
}