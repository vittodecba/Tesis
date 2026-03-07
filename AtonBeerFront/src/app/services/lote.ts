import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Lote } from '../Interfaces/lote';

@Injectable({
  providedIn: 'root'
})
export class LoteService {
  private apiUrl = 'https://localhost:5190/api/Lotes';
  constructor(private http: HttpClient) { }

  getLotes(): Observable<Lote[]> {
    return this.http.get<Lote[]>(this.apiUrl);
  }

  getLoteById(id: number): Observable<Lote> {
    return this.http.get<Lote>(`${this.apiUrl}/${id}`);
  }

// Trae solo los fermentadores que están libres
  getFermentadoresDisponibles(): Observable<any[]> {
    return this.http.get<any[]>(`https://localhost:5190/api/Fermentadores?estado=disponible`);
  }

  // Asigna un fermentador específico a un lote
  asignarFermentador(loteId: number, fermentadorId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${loteId}/fermentador`, { fermentadorId });
  }

}