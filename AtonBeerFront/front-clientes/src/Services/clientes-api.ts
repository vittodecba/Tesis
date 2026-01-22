import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CrearClienteDto {
  razonSocial: string;
  cuit: string;
  tipoCliente: string; // "Franquicia" | "Externo"
  email?: string | null;
  ubicacion: string;
  contactoNombre?: string | null;
  contactoTelefono?: string | null;
  contactoEmail?: string | null;
}

@Injectable({ providedIn: 'root' })
export class ClientesApiService {
  private baseUrl = 'http://localhost:5190/api/Clientes';

  constructor(private http: HttpClient) {}

  getAll(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl);
  }

  create(dto: CrearClienteDto): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.baseUrl, dto);
  }
}
