import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface VentaDto {
  id: number;
  numeroVenta: string;
  fechaCreacion: string;
  clienteId: number;
  clienteNombre: string;
  pedidoId: number;
  montoTotal: number;
  estadoVenta: string;
  plazo: string;
  metodoPago: string;
  metodoCobroReal: string;
  totalPagado: number;
  saldoPendiente: number;
  subtotal: number;
  descuentoMonto: number;
  descuentoPorcentaje: number;
  motivoDescuento?: string | null;

  netoGravado: number;
  ivaPorcentaje: number;
  ivaMonto: number;

  tieneFactura?: boolean;
  facturaId?: number | null;
}
export interface AplicarDescuentoDto {
  tipoDescuento: 'Porcentaje' | 'MontoFijo';
  valor: number;
  motivo?: string;
}
export interface PagosDto {
  id: number;
  ventaId: number;
  monto: number;
  fecha: string;
  metodoPago: string;
}
export interface RegistrarPagoDto {
  ventaId: number;
  monto: number;
  fecha: string;
  metodoPago: string;
}

export interface FacturaDto {
  id: number;
  ventaId: number;
  numeroVenta: string;
  tipo: string;
  numeroComprobante: string;
  fecha: string;
  clienteNombre: string;
  netoGravado: number;
  descuento: number;
  iva: number;
  total: number;
}

export interface VentaPorDia {
  fecha: string;
  total: number;
}

export interface ComparativaMes {
  diaDelPeriodo: number;
  totalActual: number;
  totalAnterior: number;
}

export interface TopCliente {
  cliente: string;
  totalComprado: number;
  cantidadVentas: number;
}

export interface TopProducto {
  producto: string;
  cantidadVendida: number;
}

export interface TopEstilo {
  estilo: string;
  cantidadVendida: number;
}

export interface EvolucionEstilo {
  fecha: string;
  estilo: string;
  cantidad: number;
}

export interface IngresoPorMes {
  mes: string;
  total: number;
}

export interface ReporteVentas {
  totalVendido: number;
  cantidadVentas: number;
  efectivoTotal: number;
  transferenciaTotal: number;
  ticketPromedio: number;
  variacionIngresosPorcentaje: number;
  ventasPorDia: VentaPorDia[];
  comparativaMensual: ComparativaMes[];
  topClientes: TopCliente[];
  topProductos: TopProducto[];
  topEstilos: TopEstilo[];
  evolucionEstilos: EvolucionEstilo[];
  evolucionMensualIngresos: IngresoPorMes[];
}

@Injectable({
  providedIn: 'root'
})
export class VentasService {
  private apiUrl = 'http://localhost:5190/api/Ventas';
  private facturasUrl = 'http://localhost:5190/api/Facturas';

  constructor(private http: HttpClient) {}

  // Genera la factura (comprobante no fiscal) para una venta entregada.
  generarFactura(ventaId: number): Observable<FacturaDto> {
    return this.http.post<FacturaDto>(`${this.facturasUrl}/generar/${ventaId}`, {});
  }

  // Descarga el PDF de la factura.
  descargarFacturaPdf(facturaId: number): Observable<Blob> {
    return this.http.get(`${this.facturasUrl}/${facturaId}/pdf`, { responseType: 'blob' });
  }

  getVentas(): Observable<VentaDto[]> {
    return this.http.get<VentaDto[]>(this.apiUrl);
  }

  patchVenta(id: number, dto: { estadoVenta?: string; plazo?: string; metodoPago?: string }): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}`, dto);
  }
  aplicarDescuento(id: number, dto: AplicarDescuentoDto): Observable<any> {
  return this.http.patch(`${this.apiUrl}/${id}/descuento`, dto);
}

  obtenerReporteVentas(fechaDesde: string, fechaHasta: string, cliente: string = '') {
    let url = `${this.apiUrl}/reporte?fechaDesde=${fechaDesde}&fechaHasta=${fechaHasta}`;
    if (cliente) {
      url += `&cliente=${encodeURIComponent(cliente)}`;
    }
    return this.http.get<ReporteVentas>(url);
  }
  descargarPdfReporte(payload: any) {
    const url = `${this.apiUrl}/reporte/pdf`;
    return this.http.post(url, payload, { responseType: 'blob' });
  }
    registrarPago(dto: RegistrarPagoDto): Observable<PagosDto> {
  return this.http.post<PagosDto>('http://localhost:5190/api/Pagos', dto);
}

getPagosPorVenta(ventaId: number): Observable<PagosDto[]> {
  return this.http.get<PagosDto[]>(`http://localhost:5190/api/Pagos/venta/${ventaId}`);
}
}
