using System;

namespace AtonBeerTesis.Application.Dtos
{
    public class PlanificacionProduccionDto
    {
        public int Id { get; set; }
        public int RecetaId { get; set; }
        public int FermentadorId { get; set; }
        public DateTime FechaProduccion { get; set; }
        public string? Observaciones { get; set; }
        public int UsuarioId { get; set; }
        public string? Estado { get; set; }
        public string? FermentadorNombre { get; set; }
    }
}