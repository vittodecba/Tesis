import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http'; 
import { Observable } from 'rxjs';
import { Lote } from '../Interfaces/lote';

@Injectable({
  providedIn: 'root'
})
export class LoteService {
  private apiUrl = 'https://localhost:5190/api/PlanProduccion'; 

  constructor(private http: HttpClient) { }

  getLotes(filtros?: any): Observable<Lote[]> {
    let params = new HttpParams();

    if (filtros) {
      if (filtros.fechaDesde) params = params.set('fechaDesde', filtros.fechaDesde);
      if (filtros.fechaHasta) params = params.set('fechaHasta', filtros.fechaHasta);
      if (filtros.recetaId) params = params.set('recetaId', filtros.recetaId.toString());
      if (filtros.fermentadorId) params = params.set('fermentadorId', filtros.fermentadorId.toString());
      if (filtros.estado) params = params.set('estado', filtros.estado);
    }

    return this.http.get<Lote[]>(this.apiUrl, { params });
  }

  getLoteById(id: number): Observable<Lote> {
    return this.http.get<Lote>(`${this.apiUrl}/${id}`);
  }

  getFermentadoresDisponibles(): Observable<any[]> {
    return this.http.get<any[]>(`https://localhost:5190/api/Fermentadores?estado=disponible`);
  }

  asignarFermentador(loteId: number, fermentadorId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${loteId}/fermentador`, { fermentadorId });
  }
}