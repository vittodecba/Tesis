import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import Swal from 'sweetalert2';
import { Router } from '@angular/router';
import { Receta, RecetaService } from '../../../services/receta';
import {FermentadorService} from '../../../services/fermentador';
import { PlanificacionService } from '../../../services/PlanificacionService';

@Component({
  selector: 'app-planificacion-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './PlanificacionComponent.html'
})
export class PlanificacionFormComponent implements OnInit {

  recetas: any[] = [];
  fermentadores: any[] = [];
  previsualizacionInsumos: any[] = []; // 👈 Lista para la tabla
  recetaSeleccionada: any = null;      // 👈 Receta actual

  nuevaPlanif = {
    recetaId: 0,
    fermentadorId: 0,
    fechaInicio: '',
    fechaFinEstimada: '',
    volumenLitros: 0,
    observaciones: '',
    usuarioId: 1,
    loteId: 0
  };

  constructor(
    private _planifService: PlanificacionService,
    private _fermentadorService: FermentadorService,
    private _recetaService: RecetaService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargarDatos();
  }

  cargarDatos() {
    this._recetaService.getAll().subscribe({
      next: (data: Receta[]) => this.recetas = data,
      error: (e: any) => console.error('Error recetas', e)
    });
    this._fermentadorService.getFermentadores().subscribe({
      next: (data: any) => this.fermentadores = data,
      error: (e: any) => console.error('Error fermentadores', e)
    });
  }

  // Se llama cuando cambia la receta o el volumen
  actualizarPrevisualizacion() {
    if (this.nuevaPlanif.recetaId === 0 || this.nuevaPlanif.volumenLitros <= 0) {
      this.previsualizacionInsumos = [];
      return;
    }

    this._recetaService.getRecetaDetalle(this.nuevaPlanif.recetaId).subscribe({
      next: (receta: any) => {
        this.recetaSeleccionada = receta;
        this.previsualizacionInsumos = receta.recetaInsumos.map((i: any) => {
          const necesario = (i.cantidad / receta.batchSizeLitros) * this.nuevaPlanif.volumenLitros;
          return {
            nombre: i.nombreInsumo,
            necesario: necesario.toFixed(2),
            stock: i.stockActual,
            unidad: i.unidadMedida,
            alcanza: necesario <= i.stockActual
          };
        });
      }
    });
  }

  enviar() {
    if (this.nuevaPlanif.recetaId === 0 || this.nuevaPlanif.fermentadorId === 0 || this.nuevaPlanif.volumenLitros <= 0) {
      Swal.fire('Atención', 'Completá los campos obligatorios y el volumen', 'warning');
      return;
    }

    // Verificar si algún insumo no alcanza antes de enviar
    const faltante = this.previsualizacionInsumos.find(i => !i.alcanza);
    if (faltante) {
      Swal.fire('Stock insuficiente', `Falta stock de ${faltante.nombre}. Necesitás ${faltante.necesario} ${faltante.unidad} y hay ${faltante.stock}`, 'warning');
      return;
    }

    this._planifService.crearPlanificacion(this.nuevaPlanif).subscribe({
      next: () => {
        Swal.fire('¡Éxito!', 'Producción planificada correctamente', 'success');
        this.router.navigate(['/planificacion/Listado']);
      },
      error: (err: any) => {
        const msg = err.error?.message || 'Error al guardar la planificación';
        Swal.fire('Error', msg, 'error');
      }
    });
  }
}