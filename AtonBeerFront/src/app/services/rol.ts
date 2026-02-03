import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface RolDto {
  id: number;
  nombre: string;
  descripcion: string;
}

@Injectable({
  providedIn: 'root',
})
export class RolService {
  private apiUrl = 'http://localhost:5190/api/Roles';

  constructor(private http: HttpClient) {}

  // 1. OBTENER (GET)
  getRoles(): Observable<RolDto[]> {
    return this.http.get<RolDto[]>(this.apiUrl);
  }

  // 2. CREAR (POST)
  crearRol(rol: Omit<RolDto, 'id'>): Observable<any> {
    return this.http.post(this.apiUrl, rol);
  }

  // 3. EDITAR (PUT)
  editarRol(id: number, rol: RolDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, rol);
  }

  // 4. ELIMINAR (DELETE)
  eliminarRol(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
