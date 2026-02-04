import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InsumoService {
  // Asegurate de que este puerto sea el que te funcionó (5190 sin 's')
  private apiUrl = 'http://localhost:5190/api/Insumo';

  constructor(private http: HttpClient) { }

  // ESTA ES LA FUNCIÓN QUE TE FALTABA
  obtenerInsumos(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  crearInsumo(insumo: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, insumo);
  }
}