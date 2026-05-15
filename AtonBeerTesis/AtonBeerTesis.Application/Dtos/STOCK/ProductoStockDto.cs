namespace AtonBeerTesis.Application.Dtos.STOCK
{
    public class ProductoStockDto
    {
        public int Id { get; set; }
        public string Estilo { get; set; } = string.Empty;
        public int? RecetaId { get; set; }
        public string? RecetaNombre { get; set; }
        public decimal StockActual { get; set; }
    }
}
