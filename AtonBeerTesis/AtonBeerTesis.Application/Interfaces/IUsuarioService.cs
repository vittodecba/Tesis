using AtonBeerTesis.Dtos;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<UsuarioDto>> GetAllAsync();
        Task<UsuarioDto?> GetByIdAsync(int id);

        // Crear usuario nuevo
        Task<UsuarioDto> CreateAsync(UsuarioCreateDto dto);

        // Editar usuario
        Task UpdateAsync(int id, UsuarioUpdateDto dto);

        // Activar/Desactivar (Baja Lógica)
        Task DeleteAsync(int id);
    }
}