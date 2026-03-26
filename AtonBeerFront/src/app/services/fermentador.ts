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

  crearFermentador(fermentador: Partial<Fermentador>): Observable<Fermentador> {
    return this.http.post<Fermentador>(this.apiUrl, fermentador);
  }

  actualizarFermentador(id: number, datos: Partial<Fermentador>): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}`, datos);
  }

  eliminarFermentador(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
