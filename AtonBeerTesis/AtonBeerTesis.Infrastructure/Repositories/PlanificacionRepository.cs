using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class PlanificacionRepository : IPlanificacionRepository
    {
        private readonly ApplicationDbContext _context;
        public PlanificacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PlanificacionProduccion> CreateAsync(PlanificacionProduccion planificacion)
        {
            await _context.PlanificacionProduccion.AddAsync(planificacion);
            await _context.SaveChangesAsync();
            return planificacion;
        }

        public async Task<bool> ExisteFermentadorOcupado(int fermentadorId, DateTime inicio, DateTime fin, int excluirLoteId = 0)
        {
            return await _context.PlanificacionProduccion.AnyAsync(
                //Validaciones de fechas para verificar si el fermentador ya está ocupado en el rango de fechas indicado.
                p => p.FermentadorId == fermentadorId &&// Verifica que el fermentador sea el mismo
                p.LoteId != excluirLoteId &&
                p.Estado != 0 && // Verifica que la planificación no esté cancelada o finalizada.
                (
                //Verifico si la fecha de inicio o fin de la nueva planificación coincide con alguna planificación
                //existente para el mismo fermentador.
                (inicio >= p.FechaInicio && inicio <= p.FechaFinEstimada) ||
                (fin > p.FechaInicio && fin <= p.FechaFinEstimada) ||
                (inicio <= p.FechaInicio && fin >= p.FechaFinEstimada)
                )
                );
        }

        public async Task<IEnumerable<PlanificacionProduccion>> GetAllAsync()
        {
            return await _context.PlanificacionProduccion
                // .Include(p => p.FermentadorPrueba)
                .Include(p => p.Fermentador)
                .Include(p => p.Lote)
                .ThenInclude(l => l.Receta) // Incluye la receta asociada al lote
                .OrderByDescending(p => p.FechaCreacion) // Ordena por fecha de creacion, la más reciente primero
                .ToListAsync();
        }

        public async Task<PlanificacionProduccion> GetByIdAsync(int id)
        {
            return await _context.PlanificacionProduccion
                        .Include(p => p.Lote)
                       .FirstOrDefaultAsync(p => p.LoteId == id);
        }

        public async Task<PlanificacionProduccion> UpdateAsync(PlanificacionProduccion planificacion)
        {
            _context.PlanificacionProduccion.Update(planificacion);
            await _context.SaveChangesAsync();
            return planificacion;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            _context.PlanificacionProduccion.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RecetaInsumo>> GetInsumosByRecetaIdAsync(int recetaId)
        {
            return await _context.RecetaInsumos
                .Include(ri => ri.Insumo)
                    .ThenInclude(i => i.unidadMedida) // <--- ACÁ ESTÁ LA MAGIA QUE FALTABA
                .Where(ri => ri.RecetaId == recetaId)
                .ToListAsync();
        }

        public async Task<Fermentador> GetFermentadorByIdAsync(int id)
        {
            return await _context.Fermentadores.FindAsync(id);
        }

        public async Task UpdateFermentadorAsync(Fermentador fermentador)
        {
            _context.Fermentadores.Update(fermentador);
            await _context.SaveChangesAsync();
        }
    }
}