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
  tieneFactura: boolean;
  facturaId?: number | null;
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

@Injectable({
  providedIn: 'root'
})
export class VentasService {
  private apiUrl = 'http://localhost:5190/api/Ventas';
  private facturasUrl = 'http://localhost:5190/api/Facturas';

  constructor(private http: HttpClient) {}

  getVentas(): Observable<VentaDto[]> {
    return this.http.get<VentaDto[]>(this.apiUrl);
  }

  patchVenta(id: number, dto: { estadoVenta?: string; plazo?: string; metodoPago?: string }): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}`, dto);
  }

  // Genera la factura (comprobante no fiscal) de una venta entregada.
  generarFactura(ventaId: number): Observable<FacturaDto> {
    return this.http.post<FacturaDto>(`${this.facturasUrl}/generar/${ventaId}`, {});
  }

  // Descarga el PDF de la factura como blob.
  descargarFacturaPdf(facturaId: number): Observable<Blob> {
    return this.http.get(`${this.facturasUrl}/${facturaId}/pdf`, { responseType: 'blob' });
  }
}
