using System.Collections.Generic;

namespace AtonBeerTesis.Application.Dtos
{
    public class PedidoCreacionDTO
    {
        public int IdCliente { get; set; }
        public List<DetallePedidoDTO> Detalles { get; set; }
    }

    public class DetallePedidoDTO
    {
        public int ProductoStockId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}