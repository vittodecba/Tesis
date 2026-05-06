namespace AtonBeerTesis.Application.Dtos.STOCK
{
    public class CreateIngresoManualDto
    {
        public int ProductoStockId { get; set; }
        public decimal Cantidad { get; set; }
        public string Motivo { get; set; } = "Ingreso Manual";
    }
}
