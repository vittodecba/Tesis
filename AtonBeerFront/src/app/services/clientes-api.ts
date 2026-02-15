import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CrearClienteDto {
  razonSocial: string;
  cuit: string;
  tipoCliente: string;
  email?: string;
  ubicacion: string;
  contactoNombre?: string;
  contactoTelefono?: string;
  contactoEmail?: string;
}

@Injectable({
  providedIn: 'root',
})
export class ClientesApiService {  
  private baseUrl = 'http://localhost:5190/api/Clientes';
  constructor(private http: HttpClient) {}

  getAll(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl);
  }

  create(dto: CrearClienteDto): Observable<any> {
    return this.http.post(this.baseUrl, dto);
  }

  update(id: number, dto: CrearClienteDto): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}`, dto);
  }

  patch(id: number, dto: any): Observable<any> {
    return this.http.patch(`${this.baseUrl}/${id}`, dto);
  }
}
