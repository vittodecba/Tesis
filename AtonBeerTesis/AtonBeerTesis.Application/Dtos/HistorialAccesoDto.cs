using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Dtos
{
    public class HistorialAccesoDto
    {    
       
      public int Id { get; set; }
      public string Usuario { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public DateTime Fecha { get; set; }
      public bool Exitoso { get; set; }
      public string Detalles { get; set; } = string.Empty;
      public string? Ip { get; set; }
     }
}
