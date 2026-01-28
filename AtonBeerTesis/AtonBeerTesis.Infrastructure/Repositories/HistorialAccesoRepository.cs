using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entidades;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace AtonBeerTesis.Infrastructure.Repositories
{
    //Repositorio para manejar las operaciones relacionadas con el historial de accesos
    public class HistorialAccesoRepository : IHistorialAccesoRepository
    {
        private readonly ApplicationDbContext _context;
        public HistorialAccesoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        //Agrega un nuevo registro de historial de acceso
        public async Task AddAsync(HistorialAcceso historialAcceso)
        {
            await _context.historialAccesos.AddAsync(historialAcceso);//Agrega un nuevo registro a la tabla HistorialAccesos
            await _context.SaveChangesAsync();//Guarda los cambios en la base de datos
        }
        //Obtiene todos los registros de historial de acceso
        public async Task<IEnumerable<HistorialAcceso>> GetAllAsync()
        {
            return await _context.historialAccesos
            .Include(u => u.Usuario) //Incluye los datos relacionados del usuario
            .OrderByDescending(h => h.FechaIntento) //Ordena los registros por fecha de acceso en orden descendente
            .ToListAsync();//Muestra todos los registros de la tabla HistorialAccesos
        }

        public async Task<IEnumerable<HistorialAcceso>> ObtenerHistorialAsync(string? email, DateTime? fecha, bool? exito)
        {
            var query = _context.historialAccesos
                .Include(h => h.Usuario)
                .AsQueryable();
            //Filtro 1: Filtrar por email si se proporciona
            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(h => h.EmailIntentado.Contains(email));
            }

            // Filtro 2: Por Fecha exacta
            if (fecha.HasValue)
            {
                // Obtener el rango de fechas para el día especificado
                var fechaInicio = fecha.Value.Date;//La fecha sin la hora
                var fechaFin = fechaInicio.AddDays(1);//El siguiente día
                query = query.Where(h => h.FechaIntento >= fechaInicio && h.FechaIntento < fechaFin);//Filtro entre las dos fechas
            }

            
            if (exito.HasValue)
            {
                query = query.Where(h => h.Exitoso == exito.Value);
            }

            // Ordenamos del más reciente al más antiguo
            return await query.OrderByDescending(h => h.FechaIntento).ToListAsync();
        }
    }
}
