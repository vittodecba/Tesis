using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IEmpresaRepository
    {
        // Devuelve la única fila de configuración del emisor (o null si no existe).
        Task<Empresa?> GetAsync();
        Task UpdateAsync(Empresa empresa);
    }
}
