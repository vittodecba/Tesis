import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HistorialService } from '../../../../core/services/historial.service';

@Component({
  selector: 'app-historial',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './historial.component.html'
})
export class HistorialComponent implements OnInit {
  private historialService = inject(HistorialService);
  historial: any[] = [];

  filtros = {
    email: '',
    fecha: '',
    exito: ''
  };

  ngOnInit(): void {
    this.aplicarFiltros();
  }

  aplicarFiltros(): void {
    this.historialService.getHistorial(this.filtros).subscribe({
      next: (res: any) => {
        // Accedemos al segundo nivel de data según tu Swagger
        console.log('📦 DATOS OK', res); // <--- AGREGÁ ESTO
        this.historial = res.data || [];
      },
      error: (err) => console.error('Error:', err)
    });
  }

  limpiar(): void {
    this.filtros = { email: '', fecha: '', exito: '' };
    this.aplicarFiltros();
  }
}