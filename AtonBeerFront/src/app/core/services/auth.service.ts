import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, BehaviorSubject, map } from 'rxjs';
import {
  UsuarioRegistro,
  UsuarioLogin,
  LoginResponse,
  UsuarioResponse,
} from '../models/usuario.models';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private readonly API_URL = 'http://localhost:5190/api/Usuario';
  private readonly TOKEN_KEY = 'aton_token';
  private readonly USER_KEY = 'aton_user';

  // Cambiamos esto para que por defecto sea null y solo lea del storage si es necesario
  private currentUserSubject = new BehaviorSubject<UsuarioResponse | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor() {
    // Solo cargamos el usuario si el token existe
    const user = this.getStoredUser();
    if (user && this.getToken()) {
      this.currentUserSubject.next(user);
    }
  }

  register(datos: UsuarioRegistro): Observable<any> {
    return this.http.post(`${this.API_URL}/registro`, datos);
  }

  login(credenciales: UsuarioLogin): Observable<LoginResponse> {
    return this.http.post<any>(`${this.API_URL}/login`, credenciales).pipe(
      map((response) => {
        const nivel1 = response.data || response;
        const nivel2 = nivel1.data || nivel1;
        const usuarioObj = nivel2.usuario || nivel2.Usuario || nivel1.usuario || {};

        return {
          token: nivel2.token || nivel2.Token || nivel1.token || '',
          usuario: {
            id: usuarioObj.id || usuarioObj.Id || 0,
            nombre: usuarioObj.nombre || usuarioObj.Nombre || 'Usuario',
            apellido: usuarioObj.apellido || usuarioObj.Apellido || '',
            email: usuarioObj.email || usuarioObj.Email || '',
            rolId: usuarioObj.rolId || usuarioObj.RolId || 0,
            rolNombre: usuarioObj.rolNombre,
          },
        } as LoginResponse;
      }),
      tap((dataAdaptada) => {
        if (dataAdaptada.token) {
          this.setSession(dataAdaptada);
        }
      }),
    );
  }

  recuperarContrasena(email: string): Observable<any> {
    return this.http.post(`${this.API_URL}/recuperar-contrasena`, { email });
  }

  restablecerContrasena(datos: any): Observable<any> {
    return this.http.post(`${this.API_URL}/restablecer-contrasena`, datos);
  }

  logout(): void {
    localStorage.clear(); // Limpia TODO para asegurar que no quede basura
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
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
    try {
      return userStr ? JSON.parse(userStr) : null;
    } catch (e) {
      return null;
    }
  }

  getCurrentUser(): UsuarioResponse | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
