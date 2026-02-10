import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InsumoService {
  //Puerto de Santi
  private apiUrl = 'http://localhost:5190/api/Insumo?TEST=ESTE_SERVICE'; 
  constructor(private http: HttpClient) { }

  obtenerInsumos(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  crearInsumo(insumo: any): Observable<any> {
    return this.http.post(this.apiUrl, insumo);
  }

  // --- NUEVOS MÉTODOS ---
  actualizarInsumo(id: number, insumo: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, insumo);
  }

  eliminarInsumo(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}