using AtonBeerTesis.Domain.Enums;
using System;

namespace AtonBeerTesis.Application.Dtos
{
    public class PlanificacionProduccionDto
    {
        public int Id { get; set; }
        public int RecetaId { get; set; }
        public string? RecetaNombre { get; set; }
        public string? Estilo { get; set; }
        public int VolumenLitros { get; set; }
        public int LoteId { get; set; }
        public int? FermentadorId { get; set; }
        public EstadoLote Estado { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinEstimada { get; set; }
        public string? Observaciones { get; set; }
        public int UsuarioId { get; set; }
        public string? FermentadorNombre { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}