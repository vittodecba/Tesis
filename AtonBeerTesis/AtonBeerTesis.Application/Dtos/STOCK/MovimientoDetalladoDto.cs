namespace AtonBeerTesis.Application.Dtos.STOCK
{
    public class MovimientoDetalladoDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; } = "";
        public string MotivoMovimiento { get; set; } = "";
        public decimal Cantidad { get; set; }
        public decimal StockPrevio { get; set; }
        public decimal StockResultante { get; set; }
        public string Estilo { get; set; } = "";
        public string FormatoNombre { get; set; } = "";
        public int? LoteId { get; set; }
    }
}
