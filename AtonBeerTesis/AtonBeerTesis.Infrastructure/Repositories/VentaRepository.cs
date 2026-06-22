using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
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
                .AsNoTracking()
                .AsSplitQuery()
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
        public async Task<List<Venta>> GetVentasPorRangoAsync(DateTime fechaDesde, DateTime fechaHasta)
        {
            return await _context.Ventas
                .AsNoTracking()
                .AsSplitQuery()
                .Include(v => v.Cliente)
                .Include(v => v.Pedido)
                    .ThenInclude(p => p.Detalles)
                        .ThenInclude(d => d.ProductoStock)
                            .ThenInclude(ps => ps.FormatoEnvase)
                .Include(v => v.Pedido)
                    .ThenInclude(p => p.Detalles)
                        .ThenInclude(d => d.ProductoStock)
                            .ThenInclude(ps => ps.Receta)
                .Where(v => v.FechaCreacion >= fechaDesde && v.FechaCreacion <= fechaHasta)
                .OrderByDescending(v => v.FechaCreacion)
                .ToListAsync();
        }

        public IQueryable<Venta> GetQueryable()
        {
            return _context.Ventas.AsNoTracking();
        }

        public async Task<Venta?> GetByPedidoIdAsync(int pedidoId)
        {
            return await _context.Ventas
                .FirstOrDefaultAsync(v => v.PedidoId == pedidoId);
        }

        // Una venta está impaga mientras su EstadoVenta sea Pendiente (PagoService la pone en
        // Pagado recién cuando el saldo llega a 0).
        public async Task<bool> TieneClienteVentasImpagasAsync(int clienteId)
        {
            return await _context.Ventas
                .AnyAsync(v => v.ClienteId == clienteId && v.EstadoVenta == EstadoVenta.Pendiente);
        }
    }
}