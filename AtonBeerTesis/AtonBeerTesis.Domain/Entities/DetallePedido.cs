using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtonBeerTesis.Domain.Entities
{
    [Table("DetallesPedidos")] // Forzamos el nombre exacto de la tabla
    public class DetallePedido
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; } // La columna que agregamos recién

        public int PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; }

        public int ProductoId { get; set; }
        public virtual ProductoPrueba Producto { get; set; }
    }
}