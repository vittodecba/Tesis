using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Infrastructure.Data;
using AtonBeerTesis.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly ApplicationDbContext _context;

        public PedidoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            return await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Estado)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.ProductoStock)
                .ToListAsync();
        }

        public async Task<Pedido> AddAsync(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();
            return pedido;
        }

        public async Task<Pedido?> GetByIdAsync(int id)
        {
            return await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Estado)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.ProductoStock)
                .ThenInclude(ps => ps.Receta)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.ProductoStock)
                .ThenInclude(ps => ps.FormatoEnvase)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Pedido pedido)
        {
            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();
        }

        //Chequea luego lo de abajo
        public async Task<ProductoStock?> GetProductoStockByIdAsync(int id)
        {
            return await _context.ProductosStock
                .Include(p => p.FormatoEnvase)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<decimal> ObtenerCantidadReservadaPendienteAsync(int productoStockId, int? pedidoIdExcluir = null)
        {
            var query = _context.DetallesPedidos
                .Where(d =>
                    d.ProductoStockId == productoStockId &&
                    d.Pedido.EstadoId == 1);

            if (pedidoIdExcluir.HasValue)
            {
                query = query.Where(d => d.PedidoId != pedidoIdExcluir.Value);
            }

            return await query.SumAsync(d => (decimal?)d.Cantidad) ?? 0;
        }

        public async Task AgregarMovimientoStockAsync(MovimientoStock movimiento)
        {
            await _context.MovimientosStock.AddAsync(movimiento);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TieneClientePedidosActivosAsync(int clienteId)
        {
            return await _context.Pedidos
                .AnyAsync(p => p.ClienteId == clienteId && p.EstadoId != 2 && p.EstadoId != 4);
        }

        public async Task<Dictionary<int, decimal>> ObtenerReservasPendientesPorProductoAsync()
        {
            return await _context.DetallesPedidos
        .Where(d => d.Pedido.EstadoId == 1)
        .GroupBy(d => d.ProductoStockId)
        .Select(g => new
        {
            ProductoStockId = g.Key,
            CantidadReservada = g.Sum(x => x.Cantidad)
        })
        .ToDictionaryAsync(x => x.ProductoStockId, x => (decimal)x.CantidadReservada);
        }
        public async Task<List<Pedido>> GetPedidosVencidosAsync(DateTime fechaLimite, int estadoPendienteId)
        {            
            return await _context.Pedidos
                .Where(p => p.EstadoId == estadoPendienteId
                         && p.FechaEntregaProgramada != null
                         && p.FechaEntregaProgramada < fechaLimite)
                .ToListAsync();
        }
    }
}