import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Fermentador } from '../Interfaces/fermentador';

@Injectable({
  providedIn: 'root',
})
export class FermentadorService {
  private apiUrl = 'http://localhost:5190/api/fermentador';

  constructor(private http: HttpClient) {}

  getFermentadores(): Observable<Fermentador[]> {
    return this.http.get<Fermentador[]>(this.apiUrl);
  }

  crearFermentador(fermentador: Fermentador): Observable<Fermentador> {
    return this.http.post<Fermentador>(this.apiUrl, fermentador);
  }

  // Usamos un objeto genérico para el PATCH para evitar problemas de tipos
  actualizarFermentador(id: number, datos: any): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}`, datos);
  }
}
