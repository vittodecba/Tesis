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
            => await _context.Recetas.FirstOrDefaultAsync(r => r.IdReceta == id);

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
    }
}
