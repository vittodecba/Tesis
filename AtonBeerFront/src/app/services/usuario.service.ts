import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Usuario, UsuarioCreate, UsuarioUpdate } from '../Interfaces/usuario.interface';

@Injectable({ providedIn: 'root' })
export class UsuarioService {
  private apiUrl = 'http://localhost:5190/api/Usuario';

  constructor(private http: HttpClient) {}

  getUsuarios(mostrarInactivos: boolean = false): Observable<Usuario[]> {
    return this.http.get<Usuario[]>(`${this.apiUrl}?mostrarInactivos=${mostrarInactivos}`);
  }

  createUsuario(usuario: UsuarioCreate): Observable<Usuario> {
    return this.http.post<Usuario>(this.apiUrl, usuario);
  }

  updateUsuario(id: number, usuario: UsuarioUpdate): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, usuario);
  }

  // Llama al HttpPatch para cambiar Activo true/false
  toggleActivo(id: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/toggle-activo`, {});
  }

  // Borrado
  deleteUsuario(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
