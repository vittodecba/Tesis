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

  private currentUserSubject = new BehaviorSubject<UsuarioResponse | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor() {
    const user = this.getStoredUser();
    if (user && this.getToken()) {
      this.currentUserSubject.next(user);
    }
  }

  register(datos: UsuarioRegistro): Observable<any> {
    return this.http.post(this.API_URL, datos);
  }

  login(credenciales: UsuarioLogin): Observable<LoginResponse> {
    return this.http.post<any>(`${this.API_URL}/login`, credenciales).pipe(
      map((response) => {
        // 1. Limpiamos el anidamiento del backend
        const nivel1 = response.data || response;
        const nivel2 = nivel1.data || nivel1;
        const u = nivel2.usuario || nivel2.Usuario || nivel1.usuario || {};

        // Debug para ver la respuesta entera en la consola (F12)
        console.log("RESPUESTA COMPLETA DEL BACKEND:", response);

        return {
          token: nivel2.token || nivel2.Token || nivel1.token || '',
          usuario: {
            id: u.id || u.Id || 0,
            nombre: u.nombre || u.Nombre || 'Usuario',
            apellido: u.apellido || u.Apellido || '',
            email: u.email || u.Email || '',
            // Buscamos el ID del rol donde sea
            rolId: u.rolId || u.RolId || nivel2.rolId || nivel2.RolId || 0,
            // Buscamos el Nombre del rol en todas las variantes posibles
            rolNombre: u.rolNombre || u.RolNombre || u.rol?.nombre || u.Rol?.Nombre || nivel2.rolNombre || 'Rol no enviado'
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
    localStorage.clear();
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