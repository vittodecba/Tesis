using AtonBeerTesis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IPagoRepository
    {
        Task<Pago> AddAsync(Pago pago);
        Task<IEnumerable<Pago>> GetByVentaIdAsync(int ventaId);
        Task<decimal> GetTotalPagadoByVentaIdAsync(int ventaId);
    }
}
