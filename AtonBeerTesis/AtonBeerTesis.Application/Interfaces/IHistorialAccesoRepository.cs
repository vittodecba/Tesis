using AtonBeerTesis.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IHistorialAccesoRepository
    {
        Task AddAsync(HistorialAcceso historialAcceso);
        Task<IEnumerable<HistorialAcceso>> GetAllAsync();
        Task<IEnumerable<HistorialAcceso>> ObtenerHistorialAsync(string?  email, DateTime? fecha, bool? exito);
    }
}
