using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class RecetaRepository : IRecetaRepository
    {
        private readonly ApplicationDbContext _context;

        public RecetaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Receta>> GetAllAsync(string? nombre = null, string? estilo = null, string? estado = null, string? orden = null)
        {
            var query = _context.Recetas.AsQueryable();

            // 1. Filtro por Nombre
            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(r => r.Nombre.Contains(nombre));

            // 2. Filtro por Estilo
            if (!string.IsNullOrWhiteSpace(estilo))
                query = query.Where(r => r.Estilo.Contains(estilo));

            // 3. Filtro por Estado
            if (!string.IsNullOrWhiteSpace(estado) && Enum.TryParse<AtonBeerTesis.Domain.Enums.EstadoReceta>(estado, true, out var estadoEnum))
                query = query.Where(r => r.Estado == estadoEnum);

            // 4. Ordenamiento
            query = orden switch
            {
                "az" => query.OrderBy(r => r.Nombre),
                "za" => query.OrderByDescending(r => r.Nombre),
                "fecha_asc" => query.OrderBy(r => r.FechaCreacion),
                _ => query.OrderByDescending(r => r.FechaCreacion) // "fecha_desc" por defecto
            };

            return await query.ToListAsync();
        }
        public async Task<Receta?> GetByIdAsync(int id)
        {
            return await _context.Recetas
                .Include(r => r.RecetaInsumos)
                    .ThenInclude(ri => ri.Insumo)
                        .ThenInclude(i => i.unidadMedida) // Trae la relación de unidades (Kg, L, etc.)
                .Include(r => r.PasosElaboracion)
                .FirstOrDefaultAsync(r => r.IdReceta == id);
        }
        public async Task AddAsync(Receta receta)
        {
            _context.Recetas.Add(receta);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Receta receta)
        {
            _context.Recetas.Update(receta);
            await _context.SaveChangesAsync();
        }
        //Metodo para modificar una receta ya creada, agregandole un nuevo insumo sin necesidad de modificar toda la receta,
        public async Task<bool> AddInsumoAsync(RecetaInsumo relacion)
        {
            _context.RecetaInsumos.Add(relacion);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> RemoveInsumoAsync(int idReceta, int idInsumo)
        {
            var relacion = await _context.RecetaInsumos
                .FirstOrDefaultAsync(ri => ri.RecetaId == idReceta && ri.InsumoId == idInsumo);

            if (relacion == null) return false;

            _context.RecetaInsumos.Remove(relacion);
            return await _context.SaveChangesAsync() > 0;
        }
        //1.Agregar Paso
        public async Task<PasosElaboracion> AddPasoAsync(PasosElaboracion paso)
        {
            _context.PasosElaboracion.Add(paso);
            await _context.SaveChangesAsync();
            return paso;
        }
        // 2. Editar Paso
        public async Task<bool> UpdatePasoAsync(PasosElaboracion paso)
        {
            _context.PasosElaboracion.Update(paso);
            // SaveChangesAsync devuelve la cantidad de filas afectadas
            return await _context.SaveChangesAsync() > 0;
        }

        // 3. Eliminar Paso
        public async Task<bool> DeletePasoAsync(int pasoId)
        {
            var paso = await _context.PasosElaboracion.FindAsync(pasoId);
            if (paso == null) return false;

            _context.PasosElaboracion.Remove(paso);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
