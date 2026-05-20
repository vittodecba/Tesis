namespace AtonBeerTesis.Application.Dtos.STOCK
{
    public class ProductoStockDto
    {
        public int Id { get; set; }
        public string Estilo { get; set; } = string.Empty;
        public int? RecetaId { get; set; }
        public string? RecetaNombre { get; set; }
        public string FormatoEnvaseNombre { get; set; } = string.Empty;
        public decimal CapacidadLitros { get; set; }
        public bool EsRetornable { get; set; }
        public decimal StockActual { get; set; }
        ///CHEQUEAR
        public decimal StockDisponible { get; set; }//PAra poder hacer los calculos y que me muestre la realidad en el pedido
    }
}