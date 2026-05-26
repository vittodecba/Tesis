using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.DTOs;
using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IPedidoService
    {
        Task<IEnumerable<object>> ObtenerTodosAsync();
        Task<PedidoEdicionDTO> ObtenerPorIdAsync(int id);
        Task<int> RegistrarPedidoAsync(PedidoCreacionDTO pedidoDto);
        Task<bool> ActualizarPedidoAsync(PedidoEdicionDTO pedidoDto);
        Task<bool> CancelarPedidoAsync(int id);
        Task<bool> EntregarPedidoAsync(PedidoEntregadoDto pedidoDto);
        Task<bool> DeshacerEntregaPedidoAsync(int pedidoId);
    }
}