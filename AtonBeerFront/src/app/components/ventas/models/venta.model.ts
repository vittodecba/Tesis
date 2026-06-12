export interface Venta {
  idVenta: number;
  fechaVenta: string;
  clienteNombre: string;
  total: number;
  estadoPago: string;
  metodoPago: string;
}