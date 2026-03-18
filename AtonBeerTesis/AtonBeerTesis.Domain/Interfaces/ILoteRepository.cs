using AtonBeerTesis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface ILoteRepository
    {
        Task<Lote> CreateAsync(Lote lote);
        Task<Lote> GetByIdAsync(int id);
        //Para obtener los insumos asociados a la receta del lote, lo que es necesario para validar el stock antes de planificar la producción.
        Task<IEnumerable<RecetaInsumo>> GetRecetaInsumosByLoteIdAsync(int loteId);
    }
}
