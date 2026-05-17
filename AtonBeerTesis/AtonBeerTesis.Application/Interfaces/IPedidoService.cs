using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.DTOs;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IPedidoService
    {
        Task<IEnumerable<object>> ObtenerTodosAsync();
        Task<int> RegistrarPedidoAsync(PedidoCreacionDTO pedidoDto);
        Task<bool>ActualizarPedidoAsync(PedidoEdicionDTO pedidoDto);
    }
}