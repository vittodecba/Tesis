namespace AtonBeerTesis.Application.Dtos.STOCK
{
    public class FormatoEnvaseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal CapacidadLitros { get; set; }
        public List<ProductoStockDto> Productos { get; set; } = new();
    }

    public class CreateFormatoEnvaseDto
    {
        public string Nombre { get; set; } = string.Empty;
        public decimal CapacidadLitros { get; set; }
    }
}
