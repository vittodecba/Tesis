namespace AtonBeerTesis.Domain.Entities
{
    public class ProductoStock
    {
        public int Id { get; set; }
        public int FormatoEnvaseId { get; set; }
        public FormatoEnvase FormatoEnvase { get; set; } = null!;
        public string Estilo { get; set; } = string.Empty;
        public decimal StockActual { get; set; }
        public ICollection<MovimientoStock> Movimientos { get; set; } = new List<MovimientoStock>();
    }
}
