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
                join l in _context.Lotes on m.LoteId equals l.Id into lotes
                from lote in lotes.DefaultIfEmpty()
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
                    FormatoNombre = f.Nombre,
                    LoteId = m.LoteId,
                    LoteCodigo = lote != null ? lote.CodigoLote : null
                }
            ).ToListAsync();
        }
    }
}
