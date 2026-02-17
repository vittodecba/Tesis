import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Receta {
  idReceta: number;
  codigo?: string;
  nombre: string;
  estilo: string;
  version: string;
  fechaCreacion: Date;       // <--- Agregá esta línea
  fechaActualizacion: Date;
  estado: string;
  batchSizeLitros?: number;
  notas?: string;
}

@Injectable({
  providedIn: 'root'
})
export class RecetaService {
  private apiUrl = 'http://localhost:5190/api/Recetas';

  constructor(private http: HttpClient) { }

  getAll(nombre?: string, estilo?: string, estado?: string, orden?: string): Observable<Receta[]> {
    let params = new HttpParams();
    if (nombre) params = params.set('nombre', nombre);
    if (estilo) params = params.set('estilo', estilo);
    if (estado) params = params.set('estado', estado);
    if (orden) params = params.set('orden', orden);

    return this.http.get<Receta[]>(this.apiUrl, { params });
  }

  // --- MÉTODO NUEVO PARA CREAR ---
  create(receta: any): Observable<any> {
    return this.http.post(this.apiUrl, receta);
  }
}