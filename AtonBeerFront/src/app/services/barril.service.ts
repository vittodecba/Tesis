import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface BarrilDto {
  id: number;
  codigo: string;
  formatoEnvaseId: number;
  nombreFormato: string;
  capacidadLitros: number;
  estado: number;
  estadoTexto: string;
  clienteId: number | null;
  clienteNombre: string | null;
  fechaAdquisicion: string;
  ultimaActualizacion: string | null;
  observaciones: string | null;
}

export interface MovimientoItemDto {
  fecha: string;
  estadoAnterior: string;
  estadoNuevo: string;
  motivo: string;
  origenDestino: string | null;
  loteId: number | null;
}

export interface BarrilDetalleDto {
  id: number;
  codigo: string;
  formato: string;
  capacidad: number;
  estado: string;
  ubicacionActual: string | null;
  observaciones: string | null;
  movimientos: MovimientoItemDto[];
}

export interface ClienteSimpleDto {
  idCliente: number;
  razonSocial: string;
}

export interface CreateBarrilDto {
  codigo: string;
  formatoEnvaseId: number;
  fechaAdquisicion: string;
  observaciones: string | null;
}

export interface UpdateBarrilDto {
  estado?: number;
  clienteId?: number | null;
  desasociarCliente?: boolean;
  fechaAdquisicion?: string;
  observaciones: string | null;
}

export interface FormatoRetornableDto {
  id: number;
  nombre: string;
  capacidadLitros: number;
  esRetornable: boolean;
}

@Injectable({ providedIn: 'root' })
export class BarrilService {
  private readonly apiBarril = 'http://localhost:5190/api/Barril';
  private readonly apiFormato = 'http://localhost:5190/api/FormatoEnvase';

  constructor(private http: HttpClient) {}

  getBarriles(): Observable<BarrilDto[]> {
    return this.http.get<BarrilDto[]>(this.apiBarril);
  }

  getDetalleBarril(id: number): Observable<BarrilDetalleDto> {
    return this.http.get<BarrilDetalleDto>(`${this.apiBarril}/${id}`);
  }

  crearBarril(dto: CreateBarrilDto): Observable<BarrilDto> {
    return this.http.post<BarrilDto>(this.apiBarril, dto);
  }

  actualizarBarril(id: number, dto: UpdateBarrilDto): Observable<void> {
    return this.http.patch<void>(`${this.apiBarril}/${id}`, dto);
  }

  eliminarBarril(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBarril}/${id}`);
  }

  getClientes(): Observable<ClienteSimpleDto[]> {
    return this.http.get<ClienteSimpleDto[]>('http://localhost:5190/api/Clientes');
  }

  getFormatosRetornables(): Observable<FormatoRetornableDto[]> {
    return this.http.get<FormatoRetornableDto[]>(this.apiFormato);
  }

  getBarrilById(id: number) {
  return this.http.get<any>(`http://localhost:5190/api/Barril/${id}`);
}

updateObservaciones(barril: any) {
  return this.http.put<any>(`http://localhost:5190/api/Barril`, barril);
}
}