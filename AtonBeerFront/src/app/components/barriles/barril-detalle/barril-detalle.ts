import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; 
import { ActivatedRoute } from '@angular/router';
import { BarrilService } from '../../../services/barril.service';
import { LucideAngularModule, Pencil, Check, X } from 'lucide-angular';

@Component({
  selector: 'app-barril-detalle',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './barril-detalle.html',
})
export class BarrilDetalleComponent implements OnInit {
  readonly Pencil = Pencil;
  readonly Check = Check;
  readonly X = X;

  barril: any = null;
  loading = true;
  editandoObs = false;
  tempObs = '';
  guardando = false;

  constructor(private route: ActivatedRoute, private _service: BarrilService) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this._service.getBarrilById(Number(id)).subscribe({
        next: (data: any) => {
          this.barril = data;
          this.tempObs = data.observaciones;
          this.loading = false;
        },
        error: () => this.loading = false
      });
    }
  }

  activarEdicion() {
    this.tempObs = this.barril.observaciones;
    this.editandoObs = true;
  }

  cancelarEdicion() {
    this.editandoObs = false;
  }

  guardarCambios() {
  this.guardando = true;
  
  // Creamos el body con todos los campos y la obs nueva
  const body = { ...this.barril, observaciones: this.tempObs };

  this._service.updateObservaciones(body).subscribe({
    next: () => {
      this.barril.observaciones = this.tempObs;
      this.editandoObs = false;
      this.guardando = false;
    },
    error: (err) => {
      console.error('Error detallado:', err);
      alert('Error al guardar. Revisá la consola (F12) para ver qué campo falta.');
      this.guardando = false;
    }
  });
}
  volver() { window.history.back(); }
}