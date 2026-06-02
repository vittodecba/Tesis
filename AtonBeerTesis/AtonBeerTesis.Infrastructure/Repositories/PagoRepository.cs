using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Infrastructure.Repositories
{   
    public class PagoRepository : IPagoRepository
    {
        private readonly ApplicationDbContext _Context;

        public PagoRepository(ApplicationDbContext applicationDbContext)
        {
           _Context = applicationDbContext;
        }
        public async Task<Pago> AddAsync(Pago pago)
        {
            _Context.Pagos.Add(pago);
            await _Context.SaveChangesAsync();
            return pago;
        }

        public async Task<IEnumerable<Pago>> GetByVentaIdAsync(int ventaId)
        {
            return await _Context.Pagos
            .Where(p => p.VentaId == ventaId)
            .OrderByDescending(p => p.Fecha)
            .ToListAsync();
        }

        public async Task<decimal> GetTotalPagadoByVentaIdAsync(int ventaId)
        {
            return await _Context.Pagos
            .Where(p => p.VentaId == ventaId)
            .SumAsync(p => (decimal?)p.Monto) ?? 0;//Esto para que no tire error si no hay pagos, devuelve 0 en ese caso
        }
    }
}
