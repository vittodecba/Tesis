import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RolService {

  // Tu puerto 7118
  private apiUrl = 'https://localhost:7118/api/Roles';

  constructor(private http: HttpClient) { }

  // 1. OBTENER (GET)
  getRoles(): Observable<any> {
    return this.http.get(this.apiUrl);
  }

  // 2. CREAR (POST)
  crearRol(rol: any): Observable<any> {
    return this.http.post(this.apiUrl, rol);
  }

  // 3. EDITAR (PUT)
  editarRol(id: number, rol: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, rol);
  }

  // 4. ELIMINAR (DELETE)
  eliminarRol(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}