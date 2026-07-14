import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface LoteCumplimiento {
  codigoLote: string;
  receta: string | null;
  estilo: string | null;
  diasEstimados: number;
  diasReales: number;
  desvioDias: number;
  estado: string;
}

export interface ReporteCumplimiento {
  lotesFinalizados: number;
  porcentajeATiempo: number;
  desvioPromedioDias: number;
  tasaDescarte: number;
  detalle: LoteCumplimiento[];
}

@Injectable({
  providedIn: 'root',
})
export class CumplimientoReporteService {
  private apiUrl = 'http://localhost:5190/api/Lote';

  constructor(private http: HttpClient) {}

  obtenerReporte(fechaDesde: string, fechaHasta: string): Observable<ReporteCumplimiento> {
    const url = `${this.apiUrl}/reporte-cumplimiento?fechaDesde=${fechaDesde}&fechaHasta=${fechaHasta}`;
    return this.http.get<ReporteCumplimiento>(url);
  }

  descargarPdf(payload: any): Observable<Blob> {
    const url = `${this.apiUrl}/reporte-cumplimiento/pdf`;
    return this.http.post(url, payload, { responseType: 'blob' });
  }
}
