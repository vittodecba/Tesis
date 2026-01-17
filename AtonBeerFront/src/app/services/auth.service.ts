import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = 'https://localhost:7118/api'; 

  constructor(private http: HttpClient) { }

  // 1. Enviar solicitud de recuperación (
  recuperarContrasena(email: string): Observable<any> {
    // Va en el BODY { email: email }
    return this.http.post(`${this.apiUrl}/Auth/recuperar-contrasena`, { email: email });
  }

  // 2. Enviar nueva contraseña (Con el token)
  restablecerContrasena(datos: any): Observable<any> {
    // datos tiene que tener: { email, token, nuevaPassword }
    return this.http.post(`${this.apiUrl}/Auth/restablecer-contrasena`, datos);
  }
}