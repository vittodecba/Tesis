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
  totalPagado: number;
  saldoPendiente: number;
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

@Injectable({
  providedIn: 'root'
})
export class VentasService {
  private apiUrl = 'http://localhost:5190/api/Ventas';

  constructor(private http: HttpClient) {}

  getVentas(): Observable<VentaDto[]> {
    return this.http.get<VentaDto[]>(this.apiUrl);
  }

  patchVenta(id: number, dto: { estadoVenta?: string; plazo?: string; metodoPago?: string }): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}`, dto);
  }

  //Metodos - PAGO//
  registrarPago(dto: RegistrarPagoDto): Observable<PagosDto> {
  return this.http.post<PagosDto>('http://localhost:5190/api/Pagos', dto);
}

getPagosPorVenta(ventaId: number): Observable<PagosDto[]> {
  return this.http.get<PagosDto[]>(`http://localhost:5190/api/Pagos/venta/${ventaId}`);
}
}
