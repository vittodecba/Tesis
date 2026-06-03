using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class VentaRepository : IVentaRepository
    {
        private readonly ApplicationDbContext _context;

        public VentaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Venta> AddAsync(Venta venta)
        {
            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();
            return venta;
        }

        public async Task UpdateAsync(Venta venta)
        {
            _context.Ventas.Update(venta);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Venta>> GetAllAsync()
        {
            return await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Pedido)
                    .ThenInclude(p => p.Detalles)
                        .ThenInclude(d => d.ProductoStock)
                            .ThenInclude(ps => ps.FormatoEnvase)
                .Include(v => v.Pedido)
                    .ThenInclude(p => p.Detalles)
                        .ThenInclude(d => d.ProductoStock)
                            .ThenInclude(ps => ps.Receta)
                .OrderByDescending(v => v.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Venta?> GetByIdAsync(int id)
        {
            return await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Pedido)
                    .ThenInclude(p => p.Detalles)
                        .ThenInclude(d => d.ProductoStock)
                            .ThenInclude(ps => ps.FormatoEnvase)
                .Include(v => v.Pedido)
                    .ThenInclude(p => p.Detalles)
                        .ThenInclude(d => d.ProductoStock)
                            .ThenInclude(ps => ps.Receta)
                .FirstOrDefaultAsync(v => v.Id == id);
        }
    }
}