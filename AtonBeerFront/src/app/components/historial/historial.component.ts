import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HistorialService } from '../../core/services/historial.service';
import { HistorialItem, HistorialFiltros } from '../../core/models/historial.models';

@Component({
  selector: 'app-historial',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './historial.component.html'
})
export class HistorialComponent implements OnInit {
  private historialService = inject(HistorialService);
  historial: HistorialItem[] = [];

  // Inicializamos el filtro correctamente
  filtros: HistorialFiltros = {
    email: '',
    fecha: '',
    exito: null
  };
  exitoSelect: string = "";
  ngOnInit(): void {
    this.aplicarFiltros();
  }

  aplicarFiltros(): void {    
    if(this.exitoSelect === 'true') this.filtros.exito = true;
    else if(this.exitoSelect === 'false') this.filtros.exito = false;
    else this.filtros.exito = null;
    this.historialService.getHistorial(this.filtros).subscribe({
      next: (res: any) => {       
        this.historial = res?.data?.data || res?.data || res || []; 
        console.log('Datos recibidos:', this.historial);
      },
      error: (err) => console.error('Error cargando historial:', err)
    });
  }
  limpiar(): void {
    this.filtros = { email: '', fecha: '', exito: null };
    this.exitoSelect = "";
    this.aplicarFiltros();
  }
}