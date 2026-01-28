import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HistorialItem, HistorialFiltros } from '../models/historial.models'; 
@Injectable({
  providedIn: 'root'
})
export class HistorialService {
  private http = inject(HttpClient); 
  
  private readonly API_URL = 'https://localhost:7118/api/Usuario/HistorialAcceso';

  getHistorial(filtros?: HistorialFiltros): Observable<any> {
    let params = new HttpParams();

    if (filtros) {
      if (filtros.email) params = params.set('email', filtros.email);
      if (filtros.fecha) params = params.set('fecha', filtros.fecha);
      
      // Manejo de strings para el booleano
      if (String(filtros.exito) === 'true') params = params.set('exito', 'true');
      if (String(filtros.exito) === 'false') params = params.set('exito', 'false');
    }
    console.log('📡 Consultando Historial:', this.API_URL, params.toString());   
    
    return this.http.get<any>(this.API_URL, { params });
  }
}