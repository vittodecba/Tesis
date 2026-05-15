using System;
using System.Collections.Generic;

namespace AtonBeerTesis.Application.Dtos.BARRIL
{
    public class BarrilDetalleDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Formato { get; set; } = string.Empty;
        public double Capacidad { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? UbicacionActual { get; set; }
        public string? Observaciones { get; set; }
        public int? LoteId { get; set; }
        public string? Estilo { get; set; }
        public string? Receta { get; set; }
        public List<MovimientoItemDto> Movimientos { get; set; } = new();
    }

    public class MovimientoItemDto
    {
        public DateTime Fecha { get; set; }
        public string EstadoAnterior { get; set; } = string.Empty;
        public string EstadoNuevo { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public string? OrigenDestino { get; set; }
        public int? LoteId { get; set; }
    }
}