using AtonBeerTesis.Domain.Entities;
namespace AtonBeerTesis.Domain.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        // Tarea 324: Listar usuarios
        Task<List<Usuario>> GetAllAsync();

        // Buscar uno solo (para editar)
        Task<Usuario?> GetByIdAsync(int id);

        // Tarea 327: Validacion de email unico
        Task<Usuario?> GetByEmailAsync(string email);

        // Tarea 328: Crear y Asignar Rol (al crear)
        Task AddAsync(Usuario usuario);

        // Tarea 325 y 326: Editar y Activar/Desactivar
        Task UpdateAsync(Usuario usuario);
    }
}