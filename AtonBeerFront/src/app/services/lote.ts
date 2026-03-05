import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Lote } from '../Interfaces/lote';

@Injectable({
  providedIn: 'root'
})
export class LoteService {
  private apiUrl = 'https://localhost:5190/api/Lotes'; 

  constructor(private http: HttpClient) { }

  getLotes(): Observable<Lote[]> {
    return this.http.get<Lote[]>(this.apiUrl);
  }
}