import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, BehaviorSubject, map } from 'rxjs';
// Ajusta esta ruta si tus modelos están en otro lado, pero suele ser así:
import { UsuarioRegistro, UsuarioLogin, LoginResponse, UsuarioResponse } from '../core/models/usuario.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  
  private readonly API_BASE = 'https://localhost:7118/api';
  private readonly TOKEN_KEY = 'aton_token';
  private readonly USER_KEY = 'aton_user';
  
  private currentUserSubject = new BehaviorSubject<UsuarioResponse | null>(this.getStoredUser());
  public currentUser$ = this.currentUserSubject.asObservable();

  register(datos: UsuarioRegistro): Observable<any> {
    return this.http.post(`${this.API_BASE}/Usuario/registro`, datos);
  }

  login(credenciales: UsuarioLogin): Observable<LoginResponse> {
    // Si tu Swagger dice Auth, cambia Usuario por Auth aquí
    return this.http.post<any>(`${this.API_BASE}/Usuario/login`, credenciales).pipe(
      map(response => {
         const nivel1 = response.data || response;
         const nivel2 = nivel1.data || nivel1; 
         const usuarioObj = nivel2.usuario || nivel2.Usuario || nivel1.usuario || {};

         return {
           token: nivel2.token || nivel2.Token || nivel1.token || "",
           usuario: {
             id: usuarioObj.id || usuarioObj.Id || 0,
             nombre: usuarioObj.nombre || usuarioObj.Nombre || "Usuario",
             apellido: usuarioObj.apellido || usuarioObj.Apellido || "",
             email: usuarioObj.email || usuarioObj.Email || "",
             rolId: usuarioObj.rolId || usuarioObj.RolId || 0,
             rolNombre: usuarioObj.rolNombre
           }
         } as LoginResponse;
      }),
      tap(dataAdaptada => {
         if (dataAdaptada.token) {
           this.setSession(dataAdaptada);
         }
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  recuperarContrasena(email: string): Observable<any> {
    return this.http.post(`${this.API_BASE}/Auth/recuperar-contrasena`, { email: email });
  }

  restablecerContrasena(datos: any): Observable<any> {
    return this.http.post(`${this.API_BASE}/Auth/restablecer-contrasena`, datos);
  }

  private setSession(authResult: LoginResponse): void {
    localStorage.setItem(this.TOKEN_KEY, authResult.token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(authResult.usuario));
    this.currentUserSubject.next(authResult.usuario);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private getStoredUser(): UsuarioResponse | null {
    const userStr = localStorage.getItem(this.USER_KEY);
    return userStr ? JSON.parse(userStr) : null;
  }

  getCurrentUser(): UsuarioResponse | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    return !!token;
  }
}