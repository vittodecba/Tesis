using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly ApplicationDbContext _context;

        public FacturaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Factura?> GetByIdAsync(int id)
        {
            return await _context.Facturas
                .Include(f => f.Venta)
                    .ThenInclude(v => v.Cliente)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Factura?> GetByVentaIdAsync(int ventaId)
        {
            return await _context.Facturas
                .Include(f => f.Venta)
                    .ThenInclude(v => v.Cliente)
                .FirstOrDefaultAsync(f => f.VentaId == ventaId);
        }

        // Último correlativo usado para ese tipo y punto de venta (0 si no hay ninguno).
        public async Task<int> GetMaxNumeroAsync(TipoComprobante tipo, int puntoVenta)
        {
            return await _context.Facturas
                .Where(f => f.Tipo == tipo && f.PuntoVenta == puntoVenta)
                .Select(f => (int?)f.Numero)
                .MaxAsync() ?? 0;
        }

        public async Task<Dictionary<int, int>> GetFacturaIdsPorVentaAsync()
        {
            return await _context.Facturas
                .ToDictionaryAsync(f => f.VentaId, f => f.Id);
        }

        public async Task<Factura> AddAsync(Factura factura)
        {
            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();
            return factura;
        }

        public async Task UpdateAsync(Factura factura)
        {
            _context.Facturas.Update(factura);
            await _context.SaveChangesAsync();
        }
    }
}
