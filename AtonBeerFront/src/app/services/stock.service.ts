import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ProductoStockDto {
  id: number;
  estilo: string;
  recetaId: number | null;
  recetaNombre: string | null;
  stockActual: number;
}

export interface FormatoEnvaseDto {
  id: number;
  nombre: string;
  capacidadLitros: number;
  productos: ProductoStockDto[];
}

export interface LoteDesignacionDto {
  id: number;
  formatoEnvaseId: number;
  nombreFormato: string;
  capacidadLitros: number;
  volumenAsignado: number;
  unidadesResultantes: number;
}

export interface CreateLoteDesignacionDto {
  formatoEnvaseId: number;
  volumenAsignado: number;
}

export interface MovimientoDetalladoDto {
  id: number;
  fecha: string;
  tipoMovimiento: string;
  motivoMovimiento: string;
  cantidad: number;
  stockPrevio: number;
  stockResultante: number;
  estilo: string;
  formatoNombre: string;
  capacidadLitros: number;
  loteId: number | null;
}

export interface CreateIngresoManualDto {
  productoStockId: number;
  cantidad: number;
  motivo: string;
}

@Injectable({
  providedIn: 'root',
})
export class StockService {
  private apiStock = 'http://localhost:5190/api/Stock';
  private apiFormato = 'http://localhost:5190/api/FormatoEnvase';
  private apiLote = 'http://localhost:5190/api/Lote';

  constructor(private http: HttpClient) {}

  // ── Formatos de Envase ──────────────────────────────────────────────────

  getFormatosEnvase(): Observable<FormatoEnvaseDto[]> {
    return this.http.get<FormatoEnvaseDto[]>(this.apiFormato);
  }

  crearFormatoEnvase(dto: { nombre: string; capacidadLitros: number }): Observable<FormatoEnvaseDto> {
    return this.http.post<FormatoEnvaseDto>(this.apiFormato, dto);
  }

  eliminarFormatoEnvase(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiFormato}/${id}`);
  }

  // ── Designaciones de Lote ───────────────────────────────────────────────

  getDesignacionesByLote(loteId: number): Observable<LoteDesignacionDto[]> {
    return this.http.get<LoteDesignacionDto[]>(`${this.apiLote}/${loteId}/designaciones`);
  }

  addDesignacion(loteId: number, dto: CreateLoteDesignacionDto): Observable<LoteDesignacionDto> {
    return this.http.post<LoteDesignacionDto>(`${this.apiLote}/${loteId}/designaciones`, dto);
  }

  deleteDesignacion(loteId: number, desId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiLote}/${loteId}/designaciones/${desId}`);
  }

  // ── Historial de movimientos ────────────────────────────────────────────

  getMovimientos(): Observable<MovimientoDetalladoDto[]> {
    return this.http.get<MovimientoDetalladoDto[]>(`${this.apiStock}/movimientos`);
  }

  agregarIngresoManual(dto: CreateIngresoManualDto): Observable<MovimientoDetalladoDto> {
    return this.http.post<MovimientoDetalladoDto>(`${this.apiStock}/ingresos`, dto);
  }

  corregirStock(productoStockId: number, nuevaCantidad: number): Observable<MovimientoDetalladoDto> {
    return this.http.put<MovimientoDetalladoDto>(
      `${this.apiStock}/productos/${productoStockId}/correccion`,
      { nuevaCantidad }
    );
  }

  egresoManual(dto: CreateIngresoManualDto): Observable<MovimientoDetalladoDto> {
    return this.http.post<MovimientoDetalladoDto>(`${this.apiStock}/egresos`, dto);
  }
}
