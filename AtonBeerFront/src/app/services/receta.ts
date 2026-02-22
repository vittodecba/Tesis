import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface RecetaInsumo {
  insumoId: number;
  nombreInsumo: string;
  cantidad: number;
  unidadMedida: string;
}

export interface Receta {
  idReceta: number;
  codigo?: string;
  nombre: string;
  estilo: string;
  version: string;
  fechaCreacion: Date;       
  fechaActualizacion: Date;
  estado: string;
  batchSizeLitros?: number;
  notas?: string;
  recetaInsumos: RecetaInsumo[];
  pasosElaboracion: any[]; // Agregado para recibir la lista del Backend
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

  create(receta: any): Observable<any> {
    return this.http.post(this.apiUrl, receta);
  }

  getRecetaDetalle(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}/detalle`);
  }

  update(id: number, receta: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, receta);
  }

  addInsumo(idReceta: number, datos: { insumoId: number, cantidad: number }): Observable<any> {
    return this.http.post(`${this.apiUrl}/${idReceta}/insumos`, datos);
  }

  removeInsumo(idReceta: number, idInsumo: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${idReceta}/insumos/${idInsumo}`);
  }

  // --- MÉTODOS PBI 92 (Pasos de Elaboración) ---
  addPaso(idReceta: number, paso: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/${idReceta}/pasos`, paso);
  }

  updatePaso(idReceta: number, idPaso: number, paso: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${idReceta}/pasos/${idPaso}`, paso);
  }

  deletePaso(idReceta: number, idPaso: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${idReceta}/pasos/${idPaso}`);
  }
}