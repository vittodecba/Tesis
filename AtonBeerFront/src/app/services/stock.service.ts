import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class StockService {
  // Ajustado al puerto 5190 de tu captura de Swagger
  private apiUrl = 'http://localhost:5190/api/Stock';

  constructor(private http: HttpClient) {}

  getProductos(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/productos`);
  }

  crearProducto(producto: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/productos`, producto);
  }

  getMovimientos(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/movimientos`);
  }
}
