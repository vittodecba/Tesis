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
                p => p.FermentadorId == fermentadorId &&
                p.LoteId != excluirLoteId &&
                p.Estado != 0 &&
                (
                    (inicio >= p.FechaInicio && inicio <= p.FechaFinEstimada) ||
                    (fin > p.FechaInicio && fin <= p.FechaFinEstimada) ||
                    (inicio <= p.FechaInicio && fin >= p.FechaFinEstimada)
                )
            );
        }

        public async Task<IEnumerable<PlanificacionProduccion>> GetAllAsync()
        {
            return await _context.PlanificacionProduccion
                .Include(p => p.Fermentador)
                .Include(p => p.Lote)
                    .ThenInclude(l => l.Receta)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }

        public async Task<PlanificacionProduccion> GetByIdAsync(int id)
        {
            return await _context.PlanificacionProduccion
                .Include(p => p.Lote)
                .FirstOrDefaultAsync(p => p.LoteId == id);
        }

        // ← nuevo: busca la planificación por LoteId
        public async Task<PlanificacionProduccion> GetByLoteIdAsync(int loteId)
        {
            return await _context.PlanificacionProduccion
                .FirstOrDefaultAsync(p => p.LoteId == loteId);
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
                    .ThenInclude(i => i.unidadMedida)
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