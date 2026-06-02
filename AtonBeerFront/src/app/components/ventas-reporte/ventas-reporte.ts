import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-ventas-reporte',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './ventas-reporte.html',
  styleUrl: './ventas-reporte.css',
})
export class VentasReporte {
  filtroForm: FormGroup;
  
  resumen = {
    totalVendido: 0,
    cantidadVentas: 0,
    efectivoTotal: 0,
    transferenciaTotal: 0
  };

  private fb = inject(FormBuilder);

  constructor() {
    this.filtroForm = this.fb.group({
      fechaDesde: [''],
      fechaHasta: ['']
    });
  }

  filtrarReporte(): void {
    console.log('Filtros aplicados:', this.filtroForm.value);
  }
}