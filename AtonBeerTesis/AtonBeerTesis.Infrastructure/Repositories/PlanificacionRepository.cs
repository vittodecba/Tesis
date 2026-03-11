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
    public class PlanificacionRepository : IPlanificacionRepository
    {
        private readonly ApplicationDbContext _context;
        public PlanificacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PlanificacionProduccion> CreateAsync(PlanificacionProduccion planificacion)
        {
            await _context.PlanificacionProduccion.AddAsync(planificacion);//Agrega la nueva planificación al contexto de la BD
            await _context.SaveChangesAsync();
            return planificacion;//Devuelve la planificación creada, que ahora incluye su ID generado por la BD
        }

        public async Task<bool> ExisteFermentadorOcupado(int fermentadorId, DateTime fechaProduccion)
        {
            return await _context.PlanificacionProduccion.AnyAsync(p => p.FermentadorId == fermentadorId && p.FechaProduccion.Date == fechaProduccion.Date);
            //Verifica lo de si ya hay una planificación para el mismo fermentador en la misma fecha.
        }

        public async Task<IEnumerable<PlanificacionProduccion>> GetAllAsync()
        {
            return await _context.PlanificacionProduccion
                .Include(p => p.FermentadorPrueba) // Incluye el fermentador relacionado
                .Include(p => p.Receta) // Incluye la receta relacionada
                .OrderByDescending(p => p.FechaProduccion) // Ordena por fecha de producción, la más reciente primero
                .ToListAsync();
        }

        public Task<PlanificacionProduccion> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PlanificacionProduccion> UpdateAsync(PlanificacionProduccion planificacion)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
        //RecetaInsumo para sacar los insumos que traen las recetas y poder validar que no esten vacios
        public async Task<IEnumerable<RecetaInsumo>> GetInsumosByRecetaIdAsync(int recetaId)
        {
            return await _context.RecetaInsumos
                .Include(ri => ri.Insumo)
                .Where(ri => ri.RecetaId == recetaId)
                .ToListAsync();
        }
    }
}
