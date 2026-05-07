namespace AtonBeerTesis.Application.Dtos.STOCK
{
    public class ProductoStockDto
    {
        public int Id { get; set; }
        public string Estilo { get; set; } = string.Empty;
        public decimal StockActual { get; set; }
    }
}
