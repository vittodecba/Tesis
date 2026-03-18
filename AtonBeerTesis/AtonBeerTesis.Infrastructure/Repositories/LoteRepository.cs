using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class LoteRepository : ILoteRepository
    {        
        private readonly ApplicationDbContext _context;
        public LoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Lote> CreateAsync(Lote lote)
        {
            await _context.Lotes.AddAsync(lote);
            await _context.SaveChangesAsync();
            return lote;
        }

        public async Task<Lote> GetByIdAsync(int id)
        {
            //Incluye la receta asociada al lote para que se puedan obtener los insumos necesarios para la planificación de producción.
            return await _context.Lotes.Include(l=>l.Receta).FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<RecetaInsumo>> GetRecetaInsumosByLoteIdAsync(int loteId)
        {
            //Buscamos el lote, para obtener la receta asociada, y luego obtenemos los insumos de esa receta.
            var lote = await _context.Lotes.Include(l => l.Receta).FirstOrDefaultAsync(l => l.Id == loteId);
            if(lote == null) return Enumerable.Empty<RecetaInsumo>();
            return await _context.RecetaInsumos
                .Include(ri => ri.Insumo)
                .Where(ri => ri.RecetaId == lote.RecetaId)
                .ToListAsync();
        }
    }
}
