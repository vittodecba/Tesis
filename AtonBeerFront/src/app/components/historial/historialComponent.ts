import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HistorialService, HistorialFiltros, HistorialItem } from '../../services/historialAcceso.Service';

@Component({
  selector: 'app-historial-acceso',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './historialComponent.html'
})
export class HistorialAccesoComponent implements OnInit {
  private historialService = inject(HistorialService);

  historial: HistorialItem[] = [];
  cargando = false;
  exitoSelect = '';

  filtros: HistorialFiltros = {
    email: '',
    fecha: '',
    exito: null
  };

  ngOnInit(): void {
    this.aplicarFiltros();
  }

  aplicarFiltros(): void {
    if (this.exitoSelect === 'true') this.filtros.exito = true;
    else if (this.exitoSelect === 'false') this.filtros.exito = false;
    else this.filtros.exito = null;

    this.cargando = true;

    this.historialService.getHistorial(this.filtros).subscribe({
      next: data => {
        this.historial= data;
        this.cargando = false;
      },
      error: err => {
        console.error('Error cargando historial:', err);
        this.historial = [];
        this.cargando = false;
      }
    });
  }

  limpiar(): void {
    this.filtros = { email: '', fecha: '', exito: null };
    this.exitoSelect = '';
    this.aplicarFiltros();
  }
}