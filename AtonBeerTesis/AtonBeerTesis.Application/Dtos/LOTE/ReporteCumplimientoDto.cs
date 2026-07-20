namespace AtonBeerTesis.Application.DTOs
{
    // P2 · Cumplimiento de planificación.
    // Compara los días de fermentación estimados al planificar contra los reales
    // (FechaFinReal − FechaElaboracion) para los lotes ya cerrados en el período.
    public class ReporteCumplimientoDto
    {
        // Cantidad de lotes con estado Finalizado en el rango.
        public int LotesFinalizados { get; set; }

        // % de finalizados cuya duración real fue <= la estimada (0-100).
        public double PorcentajeATiempo { get; set; }

        // Promedio del desvío en días (real − estimado) sobre los finalizados.
        // Positivo = atraso; negativo = terminaron antes.
        public double DesvioPromedioDias { get; set; }

        // Descartado / (Finalizado + Descartado), en % (0-100).
        public double TasaDescarte { get; set; }

        // Lotes cerrados en el rango que se excluyeron por duración inválida
        // (real <= 0 días: cerrados el mismo día o con fecha de cierre anterior a la
        // de elaboración). Se informa para que la UI avise y no confunda al usuario.
        public int LotesExcluidos { get; set; }

        public List<LoteCumplimientoDto> Detalle { get; set; } = new();
    }

    public class LoteCumplimientoDto
    {
        public string CodigoLote { get; set; } = string.Empty;
        public string? Receta { get; set; }
        public string? Estilo { get; set; }
        public int DiasEstimados { get; set; }
        public int DiasReales { get; set; }
        public int DesvioDias { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
