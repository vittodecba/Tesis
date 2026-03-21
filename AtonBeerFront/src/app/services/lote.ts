import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Lote } from '../Interfaces/lote';

@Injectable({
  providedIn: 'root',
})
export class LoteService {
  private apiUrl = 'http://localhost:5190/api/Lote';

  constructor(private http: HttpClient) {}

  getLotes(): Observable<Lote[]> {
    return this.http.get<Lote[]>(this.apiUrl);
  }

  getLoteById(id: number): Observable<Lote> {
    return this.http.get<Lote>(`${this.apiUrl}/${id}`);
  }

  getLoteActivoByFermentadorId(fermentadorId: number): Observable<Lote> {
    return this.http.get<Lote>(`${this.apiUrl}/activo/fermentador/${fermentadorId}`);
  }

  finalizarLote(id: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/finalizar`, {});
  }
}
