using AtonBeerTesis.Application.Dtos.STOCK;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class MovimientoStockRepository : IMovimientoStockRepository
    {
        private readonly ApplicationDbContext _context;

        public MovimientoStockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MovimientoDetalladoDto>> GetMovimientosDetalladosAsync()
        {
            return await (
                from m in _context.MovimientosStock
                join p in _context.ProductosStock on m.ProductoStockId equals p.Id
                join f in _context.FormatosEnvase on p.FormatoEnvaseId equals f.Id
                join r in _context.Recetas on p.RecetaId equals r.IdReceta into rJoin
                from r in rJoin.DefaultIfEmpty()   // LEFT JOIN
                orderby m.Fecha descending
                select new MovimientoDetalladoDto
                {
                    Id = m.Id,
                    Fecha = m.Fecha,
                    TipoMovimiento = m.TipoMovimiento,
                    MotivoMovimiento = m.MotivoMovimiento,
                    Cantidad = m.Cantidad,
                    StockPrevio = m.StockPrevio,
                    StockResultante = m.StockResultante,
                    Estilo = p.Estilo,
                    RecetaNombre = r != null ? r.Nombre : null,
                    FormatoNombre = f.Nombre,
                    CapacidadLitros = f.CapacidadLitros,
                    LoteId = m.LoteId
                }
            ).ToListAsync();
        }
    }
}
