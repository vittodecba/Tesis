using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _context;

        public ClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> GetAllAsync()
        {
            var clientes = await _context.Clientes.AsNoTracking().ToListAsync();

            var pedidosDict = await _context.Pedidos
                .Where(p => p.EstadoId == 2)
                .GroupBy(p => p.ClienteId)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Count);

            var ventasDict = await _context.Ventas
                .Where(v => v.EstadoVenta == EstadoVenta.Pagada)
                .GroupBy(v => v.ClienteId)
                .Select(g => new { Id = g.Key, Fecha = g.Max(x => x.FechaCreacion) })
                .ToDictionaryAsync(x => x.Id, x => (DateTime?)x.Fecha);

            foreach (var c in clientes)
            {
                c.TotalPedidos = pedidosDict.TryGetValue(c.IdCliente, out var pCount) ? pCount : 0;
                c.UltimaCompra = ventasDict.TryGetValue(c.IdCliente, out var vFecha) ? vFecha : null;
            }

            return clientes;
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == id);
        }

        public async Task AddAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }
    }
}