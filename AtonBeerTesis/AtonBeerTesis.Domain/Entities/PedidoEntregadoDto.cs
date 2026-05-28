using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Domain.Entities
{
    public class PedidoEntregadoDto
    {
        public int PedidoId { get; set; }
        public List<int> BarrilesIds { get; set; } = new List<int>();
        public DateTime Plazo { get; set; }
        public MetodoPago MetodoPago { get; set; }
    }
}
