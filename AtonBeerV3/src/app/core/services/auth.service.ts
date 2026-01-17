import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UsuarioRegistro } from '../models/usuario.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private readonly API_URL = 'https://localhost:7118/api';

  register(datos: UsuarioRegistro): Observable<any> {
    return this.http.post(`${this.API_URL}/Usuario/Registro`, datos);
  }
}