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

  // Método para crear una receta desde cero
  create(receta: any): Observable<any> {
    return this.http.post(this.apiUrl, receta);
  }

  // Método para obtener los datos de una receta específica
  getRecetaDetalle(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}/detalle`);
  }

  // Método para editar la información general (Nombre, Estilo, etc.)
  update(id: number, receta: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, receta);
  }

  // Tus métodos para gestionar ingredientes
  addInsumo(idReceta: number, datos: { insumoId: number, cantidad: number }): Observable<any> {
    return this.http.post(`${this.apiUrl}/${idReceta}/insumos`, datos);
  }

  removeInsumo(idReceta: number, idInsumo: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${idReceta}/insumos/${idInsumo}`);
  }
}