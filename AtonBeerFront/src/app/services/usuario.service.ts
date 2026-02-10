import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Usuario, UsuarioCreate, UsuarioUpdate } from '../Interfaces/usuario.interface';

@Injectable({
  providedIn: 'root',
})
export class UsuarioService {
  // Ejemplo de cambio en el servicio de login/registro
  //Puerto Santi
  /*private apiUrl = 'http://localhost:5190/api/Usuario';*/
  private apiUrl = 'https://localhost:7118/api/Usuario'; 
  constructor(private http: HttpClient) {}

  // 1. MODIFICADO: Ahora acepta el booleano para filtrar
  getUsuarios(mostrarInactivos: boolean = false): Observable<Usuario[]> {
    // Le agregamos el parámetro a la URL: ?mostrarInactivos=true o false
    return this.http.get<Usuario[]>(`${this.apiUrl}?mostrarInactivos=${mostrarInactivos}`);
  }

  // 2. Traer uno solo por ID (Para editar)
  getUsuario(id: number): Observable<Usuario> {
    return this.http.get<Usuario>(`${this.apiUrl}/${id}`);
  }

  // 3. Crear nuevo usuario (Task 328)
  createUsuario(usuario: UsuarioCreate): Observable<Usuario> {
    return this.http.post<Usuario>(this.apiUrl, usuario);
  }

  // 4. Editar usuario (Task 330)
  updateUsuario(id: number, usuario: UsuarioUpdate): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, usuario);
  }

  // 5. Activar/Desactivar (Task 331)
  toggleActivo(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
