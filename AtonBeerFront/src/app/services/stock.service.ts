import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class StockService {
  private apiUrl = 'http://localhost:5190/api/Stock';

  constructor(private http: HttpClient) {}

  getProductos(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/productos`);
  }

  getMovimientos(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/movimientos`);
  }

  crearProducto(producto: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/productos`, producto);
  }

  actualizarProducto(id: number, producto: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/productos/${id}`, producto);
  }

  eliminarProducto(id: number): Observable<any> {
    // OJO: Chequeá si tu apiUrl ya termina en / o no.
    // Si tu apiUrl es '.../api/Stock', necesitás el '/productos/'
    return this.http.delete(`${this.apiUrl}/productos/${id}`);
  }

  registrarMovimiento(dto: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/Registrar`, dto);
  }
}
