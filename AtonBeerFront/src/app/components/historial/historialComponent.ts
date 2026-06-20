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

  paginaActual = 1;
  itemsPorPagina = 10;
  opcionesPorPagina = [10, 25, 50];

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
        this.paginaActual = 1;
        this.cargando = false;
      },
      error: err => {
        console.error('Error cargando historial:', err);
        this.historial = [];
        this.cargando = false;
      }
    });
  }

  get totalRegistros(): number {
  return this.historial.length;
}

get totalPaginas(): number {
  return Math.max(1, Math.ceil(this.totalRegistros / this.itemsPorPagina));
}

get historialPaginado(): HistorialItem[] {
  const inicio = (this.paginaActual - 1) * this.itemsPorPagina;
  return this.historial.slice(inicio, inicio + this.itemsPorPagina);
}

get registroDesde(): number {
  if (this.totalRegistros === 0) return 0;
  return (this.paginaActual - 1) * this.itemsPorPagina + 1;
}

get registroHasta(): number {
  return Math.min(
    this.paginaActual * this.itemsPorPagina,
    this.totalRegistros
  );
}

cambiarPagina(pagina: number): void {
  if (pagina >= 1 && pagina <= this.totalPaginas) {
    this.paginaActual = pagina;
  }
}

cambiarCantidad(): void {
  this.paginaActual = 1;
}

  limpiar(): void {
    this.filtros = { email: '', fecha: '', exito: null };
    this.exitoSelect = '';
    this.aplicarFiltros();
  }
}