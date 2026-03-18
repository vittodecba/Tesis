import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs'; // Importamos 'of'

@Injectable({
  providedIn: 'root'
})
export class FermentadorPrueba {
  constructor() { }

  getFermentadores(): Observable<any[]> {
    // En lugar de usar http.get, devolvemos una lista fija (Mock)
    const datosPrueba = [
      { id: 3, nombre: '1', capacidad: 1000 },
      { id: 4, nombre: '2', capacidad: 2000 },
      { id: 5, nombre: '3', capacidad: 3000 }
    ];
    return of(datosPrueba); // 'of' lo convierte en un Observable para que el componente no note la diferencia
  }
}