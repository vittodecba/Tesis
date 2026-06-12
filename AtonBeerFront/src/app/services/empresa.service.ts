import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface EmpresaDto {
  id: number;
  razonSocial: string;
  cuit: string;
  domicilioComercial: string;
  condicionIVA: string;
  puntoVenta: number;
  ingresosBrutos: string;
  inicioActividades: string;
}

@Injectable({ providedIn: 'root' })
export class EmpresaService {
  private apiUrl = 'http://localhost:5190/api/Empresa';

  constructor(private http: HttpClient) {}

  get(): Observable<EmpresaDto> {
    return this.http.get<EmpresaDto>(this.apiUrl);
  }

  update(dto: Partial<EmpresaDto>): Observable<any> {
    return this.http.put(this.apiUrl, dto);
  }
}
