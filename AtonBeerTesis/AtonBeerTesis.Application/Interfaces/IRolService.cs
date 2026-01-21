using AtonBeerTesis.Application.DTOs;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IRolService
    {
        Task<IEnumerable<RolDto>> GetAll();
        Task<RolDto> GetById(int id);
        Task Create(RolDto rolDto);
        Task Update(int id, RolDto rolDto); // Agregamos ID para asegurar
        Task Delete(int id);
    }
}