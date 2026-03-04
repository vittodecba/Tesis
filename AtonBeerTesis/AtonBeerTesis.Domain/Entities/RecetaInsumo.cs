using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Domain.Entities
{
    public class RecetaInsumo
    {
        public int Id { get; set; }
        /// Llave foránea compuesta: RecetaId + InsumoId
        public int RecetaId { get; set; }
        public int InsumoId { get; set; }
        public decimal Cantidad { get; set; }
        public virtual Receta Receta { get; set; }
        public virtual Insumo Insumo { get; set; }
        public virtual unidadMedida unidadMedida { get; set; }
        public int unidadMedidaId { get; set; }
    }
}
