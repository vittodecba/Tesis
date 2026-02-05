namespace AtonBeerTesis.Application // O el namespace que ya tenga tu archivo
{
    public class InsumoDto
    {
        public string NombreInsumo { get; set; }

        public string Codigo { get; set; } // <--- Faltaba este

        public int TipoInsumoId { get; set; }

        public string Unidad { get; set; }

        public decimal StockActual { get; set; } // <--- Faltaba este

        public string Observaciones { get; set; }
    }
}