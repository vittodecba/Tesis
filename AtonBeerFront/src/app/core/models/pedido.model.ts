export interface PedidoCreacion {
  clienteId: number;
  fechaPedido: string;
  observaciones?: string;
  estado?: string;
  detalles: DetallePedidoCreacion[];
}

export interface DetallePedidoCreacion {
  productoStockId: number;
  cantidad: number;
  precioUnitario: number;
}

export interface Cliente {
  idCliente: number;
  razonSocial: string;
}

export interface ProductoStock {
  idProductoStock: number;
  nombre: string;
  cantidadDisponible?: number;
  stockActual?: number;
  cantidad?: number;
}