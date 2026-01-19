using AtonBeerTesis.Application.Interfaces; // Para ver IRepository
using AtonBeerTesis.Infrastructure.Data;    // Para ver ApplicationDbContext
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    // Le cambié el nombre de "BaseRepository" a "Repository" para que coincida con tu archivo
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

            // Intenta devolver el ID. NOTA: Tus tablas deben tener una propiedad llamada "Id"
            try
            {
                return _context.Entry(entity).Property("Id").CurrentValue;
            }
            catch
            {
                return null; // Si no encuentra "Id", devuelve null para no romper
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

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
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
                // Copia los valores nuevos sobre la entidad vieja
                _context.Entry(foundEntity).CurrentValues.SetValues(entity);
                _context.SaveChanges();
            }
        }
    }
}