import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UnidadMedidaService {
 
  private apiUrl = 'http://localhost:5190/api/unidadMedida';

  constructor(private http: HttpClient) { }

  getUnidades(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  crear(unidad: any): Observable<any> {
    return this.http.post(this.apiUrl, unidad);
  }

  actualizar(id: number, unidad: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, unidad);
  }

  eliminar(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}