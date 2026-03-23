import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegistroFermentacion } from '../Interfaces/registro-fermentacion';

@Injectable({
  providedIn: 'root',
})
export class RegistroFermentacionService {
  private apiUrl = 'http://localhost:5190/api/RegistroFermentacion';

  constructor(private http: HttpClient) {}

  getByLoteId(loteId: number): Observable<RegistroFermentacion[]> {
    return this.http.get<RegistroFermentacion[]>(`${this.apiUrl}/lote/${loteId}`);
  }

  crearRegistro(registro: RegistroFermentacion): Observable<RegistroFermentacion> {
    return this.http.post<RegistroFermentacion>(this.apiUrl, registro);
  }

  actualizarRegistro(id: number, datos: Partial<RegistroFermentacion>): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}`, datos);
  }

  eliminarRegistro(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
