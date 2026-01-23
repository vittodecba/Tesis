import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HistorialItem } from '../models/historial.models';

@Injectable({
  providedIn: 'root'
})
export class HistorialService {
  private http = inject(HttpClient);
  // IMPORTANTE: Verificá que esta URL sea exactamente igual a la de tu Swagger
  private readonly API_URL = 'https://localhost:7118/api/Usuario/HistorialAcceso';

  getHistorial(filtros?: any): Observable<HistorialItem[]> {
    let params = new HttpParams();

    if (filtros) {
      // Ajustamos los nombres para que coincidan con los parámetros del Backend
      if (filtros.email) params = params.set('email', filtros.email);
      if (filtros.fecha) params = params.set('fecha', filtros.fecha);
      
   
      if (filtros.exito === 'true') params = params.set('exito', 'true');
      if (filtros.exito === 'false') params = params.set('exito', 'false');
    }

   
    console.log('Llamando a la API con:', params.toString());
    
    return this.http.get<HistorialItem[]>(this.API_URL, { params });
  }
}