using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IRepository<TEntity>
    {
        object Add(TEntity entity);
        Task<object> AddAsync(TEntity entity);
        long Count(Expression<Func<TEntity, bool>> filter);
        Task<long> CountAsync(Expression<Func<TEntity, bool>> filter);
        List<TEntity> FindAll();
        Task<List<TEntity>> FindAllAsync();
        TEntity FindOne(params object[] keyValues);
        Task<TEntity> FindOneAsync(params object[] keyValues);
        void Remove(params object[] keyValues);
        void Update(object id, TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync();
    }
}
