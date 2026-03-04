using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Domain.Enums
{
    public enum EstadoFermentador
    {
        Disponible = 1,    // Listo para recibir birra
        Ocupado = 2,       // Tiene un lote adentro
        Sucio = 3,         // Terminó de fermentar, necesita limpieza CIP
        Mantenimiento = 4  // Roto o en reparación
    }
}