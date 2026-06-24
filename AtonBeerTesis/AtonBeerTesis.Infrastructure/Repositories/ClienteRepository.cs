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
            return await _context.Clientes
                .AsNoTracking()
                .Select(c => new Cliente
                {
                    IdCliente = c.IdCliente,
                    Tipocliente = c.Tipocliente,
                    EstadoCliente = c.EstadoCliente,
                    RazonSocial = c.RazonSocial,
                    Cuit = c.Cuit,
                    Email = c.Email,
                    Ubicacion = c.Ubicacion,
                    ContactoNombre = c.ContactoNombre,
                    ContactoTelefono = c.ContactoTelefono,
                    ContactoEmail = c.ContactoEmail,
                    UltimoPedido = c.UltimoPedido,
                    TotalPedidos = _context.Pedidos
                        .Count(p => p.ClienteId == c.IdCliente && p.EstadoId == 2),
                    UltimaCompra = _context.Ventas
                        .Where(v => v.ClienteId == c.IdCliente && v.EstadoVenta == EstadoVenta.Pagada)
                        .OrderByDescending(v => v.FechaCreacion)
                        .Select(v => (DateTime?)v.FechaCreacion)
                        .FirstOrDefault()
                })
                .ToListAsync();
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
