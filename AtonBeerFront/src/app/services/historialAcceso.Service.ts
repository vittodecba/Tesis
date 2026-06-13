import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface HistorialItem {
  id: number;
  usuario: string;
  email: string;
  fecha: string;
  exitoso: boolean;
  detalles: string;
  ip?: string | null;
}

export interface HistorialFiltros {
  email?: string;
  fecha?: string;
  exito?: boolean | null;
}

@Injectable({
  providedIn: 'root'
})
export class HistorialService {
  private http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5190/api/HistorialAcceso';

  getHistorial(filtros?: HistorialFiltros): Observable<HistorialItem[]> {
    let params = new HttpParams();

    if (filtros?.email) 
    {
      params = params.set('email', filtros.email);
    }

    if (filtros?.fecha)
    {
      params = params.set('fecha', filtros.fecha);
    }

    if (filtros?.exito !== null && filtros?.exito !== undefined) 
    {
      params = params.set('exito', String(filtros.exito));
    }

    return this.http.get<HistorialItem[]>(this.apiUrl, { params });
  }
}