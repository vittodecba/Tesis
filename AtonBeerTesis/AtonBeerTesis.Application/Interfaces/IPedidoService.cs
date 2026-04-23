using AtonBeerTesis.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IPedidoService
    {
        Task<int> RegistrarPedidoAsync(PedidoCreacionDTO pedidoDto);
    }
}
