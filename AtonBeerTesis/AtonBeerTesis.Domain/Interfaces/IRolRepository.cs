using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Domain.Interfaces
{
    public interface IRolRepository
    {
        Task<IEnumerable<Rol>> GetAll();
        Task<Rol> GetById(int id);
        Task Add(Rol rol);
        Task Update(Rol rol);
        Task Delete(int id);
    }
}