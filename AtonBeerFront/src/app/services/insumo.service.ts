import { Injectable } from '@angular/core'; // <--- ESTO VA ASÍ
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InsumoService {
  private apiUrl = 'http://localhost:5190/api/Insumo'; 
  private tipoUrl = 'http://localhost:5190/api/Insumo/tipos'; 

  constructor(private http: HttpClient) { }

  // --- MÉTODOS DE INSUMOS ---
  obtenerInsumos(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  crearInsumo(insumo: any): Observable<any> {
    return this.http.post(this.apiUrl, insumo);
  }

  actualizarInsumo(id: number, insumo: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, insumo);
  }

  eliminarInsumo(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  // --- MÉTODOS DE TIPOS ---
  obtenerTipos(): Observable<any[]> {
    return this.http.get<any[]>(this.tipoUrl);
  }

  // Método para crear tipo
  crearTipo(tipo: any): Observable<any> {
    return this.http.post(this.tipoUrl, tipo);
  }
  eliminarTipo(id: number): Observable<any> {
  return this.http.delete(`${this.tipoUrl}/${id}`);
}
}