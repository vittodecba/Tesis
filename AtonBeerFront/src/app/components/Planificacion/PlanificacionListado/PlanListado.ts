import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PlanificacionService } from '../../../services/PlanificacionService';
import { RecetaService, Receta } from '../../../services/receta';
import { FermentadorService } from '../../../services/fermentador';
import { LucideAngularModule, Plus, Calendar, FlaskConical, ClipboardList, Pencil, List, LayoutGrid, Search } from 'lucide-angular';
import { PlanificacionCalendarComponent } from '../PlanificacionCalendario/PlanCalendario';

@Component({
  selector: 'app-planificacion-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, LucideAngularModule, PlanificacionCalendarComponent],
  templateUrl: './PlanListado.html',
  styleUrls: ['./PlanListado.scss']
})
export class PlanificacionListComponent implements OnInit {
  planificaciones: any[] = [];
  planificacionesFiltradas: any[] = [];
  recetas: Receta[] = [];
  fermentadores: any[] = [];
  loading: boolean = true;

  // Vista: 'tabla' | 'tarjetas' | 'calendario'
  vista: string = 'tabla';

  // Filtros
  filtroEstado: string = '2';
  filtroFermentador: string = '';
  filtroOrden: string = 'reciente';
  filtroFechaDesde: string = '';
  filtroFechaHasta: string = '';

  // Modal edición
  mostrarModal: boolean = false;
  loteEditando: any = {};
  guardando: boolean = false;

  Plus = Plus;
  Calendar = Calendar;
  FlaskConical = FlaskConical;
  ClipboardList = ClipboardList;
  Pencil = Pencil;
  List = List;
  LayoutGrid = LayoutGrid;
  Search = Search;

  estadosMapping: { [key: number]: { nombre: string, clase: string, color: string } } = {
    0: { nombre: 'Cancelado',   clase: 'bg-danger',            color: '#dc3545' },
    1: { nombre: 'Planificado', clase: 'bg-primary',           color: '#0d6efd' },
    2: { nombre: 'En Proceso',  clase: 'bg-warning text-dark', color: '#ffc107' },
    3: { nombre: 'Finalizado',  clase: 'bg-success',           color: '#198754' }
  };

  constructor(
    private _planifService: PlanificacionService,
    private _recetaService: RecetaService,
    private _fermentadorService: FermentadorService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargarPlanificaciones();
    this.cargarRecetas();
    this.cargarFermentadores();
  }

  cargarRecetas() {
    this._recetaService.getAll().subscribe({
      next: (data) => this.recetas = data ?? []
    });
  }

  cargarFermentadores() {
    this._fermentadorService.getFermentadores().subscribe({
      next: (data) => this.fermentadores = data ?? []
    });
  }

  getNombreReceta(recetaId: number): string {
    const receta = this.recetas.find(r => r.idReceta === recetaId);
    return receta ? receta.nombre : `Receta #${recetaId}`;
  }

  getNombreFermentador(id: number): string {
    const f = this.fermentadores.find(f => f.id === id);
    return f ? f.nombre : `#${id}`;
  }

  aplicarFiltros() {
    let resultado = [...this.planificaciones];

    if (this.filtroEstado !== '') {
      resultado = resultado.filter(p => p.estado === Number(this.filtroEstado));
    }
    if (this.filtroFermentador !== '') {
      resultado = resultado.filter(p => p.fermentadorId === Number(this.filtroFermentador));
    }
    if (this.filtroFechaDesde) {
      resultado = resultado.filter(p => new Date(p.fechaInicio) >= new Date(this.filtroFechaDesde));
    }
    if (this.filtroFechaHasta) {
      resultado = resultado.filter(p => new Date(p.fechaInicio) <= new Date(this.filtroFechaHasta));
    }
    if (this.filtroOrden === 'reciente') {
      resultado.sort((a, b) => new Date(b.fechaInicio).getTime() - new Date(a.fechaInicio).getTime());
    } else {
      resultado.sort((a, b) => new Date(a.fechaInicio).getTime() - new Date(b.fechaInicio).getTime());
    }

    this.planificacionesFiltradas = resultado;
  }

  limpiarFiltros() {
    this.filtroEstado = '';
    this.filtroFermentador = '';
    this.filtroOrden = 'reciente';
    this.filtroFechaDesde = '';
    this.filtroFechaHasta = '';
    this.aplicarFiltros();
  }

  verDetalle(id: number) {
    this.router.navigate(['/planificacion/detalle', id]);
  }

  abrirModal(p: any) {
    this.loteEditando = {
      loteId: p.loteId,
      recetaId: p.recetaId,
      fermentadorId: p.fermentadorId,
      volumenLitros: p.volumenLitros,
      estado: p.estado,
      observaciones: p.observaciones ?? '',
      fechaInicio: p.fechaInicio ? p.fechaInicio.substring(0, 10) : '',
      fechaFinEstimada: p.fechaFinEstimada ? p.fechaFinEstimada.substring(0, 10) : ''
    };
    this.mostrarModal = true;
  }

  cerrarModal() {
    this.mostrarModal = false;
    this.guardando = false;
  }

  guardarCambios() {
    this.guardando = true;
    const datos = {
      loteId: Number(this.loteEditando.loteId),
      recetaId: Number(this.loteEditando.recetaId),
      fermentadorId: Number(this.loteEditando.fermentadorId),
      volumenLitros: Number(this.loteEditando.volumenLitros),
      estado: Number(this.loteEditando.estado),
      fechaInicio: this.loteEditando.fechaInicio,
      fechaFinEstimada: this.loteEditando.fechaFinEstimada,
      observaciones: this.loteEditando.observaciones,
      usuarioId: Number(this.loteEditando.usuarioId ?? 1)
    };
    this._planifService.actualizarPlanificacion(datos.loteId, datos).subscribe({
      next: () => {
        this.cerrarModal();
        this.cargarPlanificaciones();
      },
      error: (e) => {
        console.error('Error al guardar', e);
        alert(e.error?.message ?? 'Error al guardar los cambios');
        this.guardando = false;
      }
    });
  }

  cargarPlanificaciones() {
    this.loading = true;
    this._planifService.getPlanificaciones().subscribe({
      next: (data) => {
        this.planificaciones = data;
        this.aplicarFiltros();
        this.loading = false;
      },
      error: (e) => {
        console.error('Error al cargar planificaciones', e);
        this.loading = false;
      }
    });
  }
}