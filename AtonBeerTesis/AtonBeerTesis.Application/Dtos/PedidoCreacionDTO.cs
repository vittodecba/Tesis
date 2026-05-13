using System.Collections.Generic;

namespace AtonBeerTesis.Application.Dtos
{
    public class PedidoCreacionDTO
    {
        public int IdCliente { get; set; }
        public string Observaciones { get; set; }
        public decimal TotalPedido { get; set; }
        public List<PedidoDetalleDTO> Detalles { get; set; }
    }

    public class PedidoDetalleDTO
    {
        public int ProductoStockId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}