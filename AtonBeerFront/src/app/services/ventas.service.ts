import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface VentaDto {
  id: number;
  numeroVenta: string;
  fechaCreacion: string;
  clienteId: number;
  clienteNombre: string;
  pedidoId: number;
  montoTotal: number;
  estadoVenta: string;
  plazo: string;
  metodoPago: string;
}

export interface VentaPorDia {
  fecha: string;
  total: number;
}

export interface ComparativaMes {
  diaDelPeriodo: number;
  totalActual: number;
  totalAnterior: number;
}

export interface TopCliente {
  cliente: string;
  totalComprado: number;
  cantidadVentas: number;
}

export interface TopProducto {
  producto: string;
  cantidadVendida: number;
}

export interface TopEstilo {
  estilo: string;
  cantidadVendida: number;
}

export interface EvolucionEstilo {
  fecha: string;
  estilo: string;
  cantidad: number;
}

export interface IngresoPorMes {
  mes: string;
  total: number;
}

export interface ReporteVentas {
  totalVendido: number;
  cantidadVentas: number;
  efectivoTotal: number;
  transferenciaTotal: number;
  ticketPromedio: number;
  variacionIngresosPorcentaje: number;
  ventasPorDia: VentaPorDia[];
  comparativaMensual: ComparativaMes[];
  topClientes: TopCliente[];
  topProductos: TopProducto[];
  topEstilos: TopEstilo[];
  evolucionEstilos: EvolucionEstilo[];
  evolucionMensualIngresos: IngresoPorMes[];
}

@Injectable({
  providedIn: 'root'
})
export class VentasService {
  private apiUrl = 'http://localhost:5190/api/Ventas';

  constructor(private http: HttpClient) {}

  getVentas(): Observable<VentaDto[]> {
    return this.http.get<VentaDto[]>(this.apiUrl);
  }

  patchVenta(id: number, dto: { estadoVenta?: string; plazo?: string; metodoPago?: string }): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}`, dto);
  }

  obtenerReporteVentas(fechaDesde: string, fechaHasta: string) {
    return this.http.get<ReporteVentas>(`${this.apiUrl}/reporte`, {
      params: { fechaDesde, fechaHasta }
    });
  }
  descargarPdfReporte(fechaDesde: string, fechaHasta: string) {
    const url = `${this.apiUrl}/reporte/pdf?fechaDesde=${fechaDesde}&fechaHasta=${fechaHasta}`;
    return this.http.get(url, { responseType: 'blob' });
  }
}