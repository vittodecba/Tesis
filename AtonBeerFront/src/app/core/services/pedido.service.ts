import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PedidoService {
  private apiUrl = 'http://localhost:5190/api/Pedidos';

  constructor(private http: HttpClient) {}

  getClientes(): Observable<any[]> {
    return this.http.get<any[]>('http://localhost:5190/api/Clientes');
  }

  getProductos(): Observable<any[]> {
    return this.http.get<any[]>('http://localhost:5190/api/Stock/productos');
  }

  getPedidos(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  } 
  getPedidoPorId(id: number): Observable<any> {
  return this.http.get<any>(`${this.apiUrl}/${id}`);
}

  crearPedido(pedido: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, pedido);
  }
  actualizarPedido(id: number, pedido: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}?id=${id}`, pedido);
  }
  cancelarPedido(id: number): Observable<any> {
  return this.http.patch<any>(`${this.apiUrl}/${id}/cancelar`, {});
}

entregarPedido(id: number): Observable<any> {
  return this.http.patch<any>(`${this.apiUrl}/${id}/entregar`, {});
}
}