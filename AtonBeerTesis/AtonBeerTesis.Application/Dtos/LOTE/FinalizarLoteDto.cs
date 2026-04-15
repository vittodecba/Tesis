using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Application.DTOs
{
    public class FinalizarLoteDto
    {
        /// <summary>
        /// EstadoLote.Finalizado (3) o EstadoLote.Descartado (4).
        /// Defaults a Finalizado si no se envía.
        /// </summary>
        public EstadoLote Estado { get; set; } = EstadoLote.Finalizado;
    }
}
