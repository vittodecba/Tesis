using AtonBeerTesis.Application.Dtos; // <- para ActualizarClienteDto
using AtonBeerTesis.Application.Dtos.Cliente;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IClienteService
    {
        Task<List<ClienteDto>> GetAllAsync(string? tipo = null, string? ubicacion = null, string? estado = null);
        Task<ClienteDto?> GetByIdAsync(int id);

        Task<int> CreateAsync(CrearClienteDto dto);
        Task<bool> UpdateAsync(int id, ActualizarClienteDto dto);

        Task<bool> DeactivateAsync(int id);

        List<string> GetTiposCliente();
        List<string> GetEstadosCliente();
        Task<bool> PatchAsync(int id, PatchClienteDto dto);

    }
}
