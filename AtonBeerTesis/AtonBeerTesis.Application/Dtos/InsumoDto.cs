namespace AtonBeerTesis.Application // O el namespace que ya tenga tu archivo
{
    public class InsumoDto
    {
        public string NombreInsumo { get; set; }

        public string Codigo { get; set; } // <--- Faltaba este

        //TipoInsumo
        public int TipoInsumoId { get; set; }
        public string? TipoNombre { get; set; }
        //
        public int unidadMedidaId { get; set; }
        public string? Unidad { get; set; }//Este es para que el front vea el nombre
        public decimal StockActual { get; set; } // <--- Faltaba este

        public string Observaciones { get; set; }
        public DateTime? UltimaActualizacion { get; set; } = DateTime.Now;
    }
}