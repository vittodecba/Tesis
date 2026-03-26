import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AnyCatcher } from 'rxjs/internal/AnyCatcher';

@Injectable({
  providedIn: 'root'
})
export class PlanificacionService {
  private apiUrl = 'http://localhost:5190/api/PlanProduccion'; 

  constructor(private http: HttpClient) { }

  crearPlanificacion(datos: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, datos);
  }
  getPlanificaciones(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }
  actualizarFechas(loteId: number, fechaInicio: string, fechaFin: string | null): Observable<any> {
  return this.http.patch<any>(`${this.apiUrl}/${loteId}/fechas`, {
    fechaInicio,
    fechaFinEstimada: fechaFin
  });
}
actualizarPlanificacion(loteId: number, datos: any): Observable<any> {
  return this.http.put<any>(`${this.apiUrl}/${loteId}`, datos);
}
 eliminar(id: number): Observable<any> {
  return this.http.delete(`${this.apiUrl}/${id}`);
}
}