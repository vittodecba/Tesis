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
}
