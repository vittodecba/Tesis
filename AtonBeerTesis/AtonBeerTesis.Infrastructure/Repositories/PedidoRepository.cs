using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    using AtonBeerTesis.Application.Interfaces;
    using AtonBeerTesis.Domain.Entities;
    using AtonBeerTesis.Infrastructure.Data;
    using Microsoft.EntityFrameworkCore;

    public class PedidoRepository : IPedidoRepository
    {
        private readonly ApplicationDbContext _context;

        public PedidoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Pedido> AddAsync(Pedido pedido)
        {
            await _context.Pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync();
            return pedido;
        }
    }
}