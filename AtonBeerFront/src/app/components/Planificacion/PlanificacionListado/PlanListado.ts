import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PlanificacionService } from '../../../services/PlanificacionService';
import { RecetaService, Receta } from '../../../services/receta';
import { FermentadorService } from '../../../services/fermentador';
import { LucideAngularModule, Plus, Calendar, FlaskConical, ClipboardList, Pencil, List, LayoutGrid, Search, Trash2, Beer } from 'lucide-angular';
import { PlanificacionCalendarComponent } from '../PlanificacionCalendario/PlanCalendario';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-planificacion-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, LucideAngularModule, PlanificacionCalendarComponent],
  templateUrl: './PlanListado.html',
  styleUrls: ['./PlanListado.scss']
})
export class PlanificacionListComponent implements OnInit {
  hoy: string = new Date().toISOString().split('T')[0]; // formato yyyy-MM-dd para [min]

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
  errorVolumenEdicion: string | null= null;

  // Fermentadores visibles en el select de edición: solo Disponibles + el actualmente asignado
  get fermentadoresDisponiblesEdicion(): any[] {
    return this.fermentadores.filter(f =>
      String(f.estado) === '1' || f.id === Number(this.loteEditando?.fermentadorId)
    );
  }

  Plus = Plus;
  Calendar = Calendar;
  FlaskConical = FlaskConical;
  ClipboardList = ClipboardList;
  Pencil = Pencil;
  List = List;
  LayoutGrid = LayoutGrid;
  Search = Search;
  Trash2 = Trash2;
  Beer = Beer;
  Swal = Swal;

  estadosMapping: { [key: number]: { nombre: string, clase: string, color: string } } = {
    0: { nombre: 'Cancelado',   clase: 'bg-danger',            color: '#dc3545' },
    1: { nombre: 'Planificado', clase: 'bg-primary',           color: '#0d6efd' },
    2: { nombre: 'En Proceso',  clase: 'bg-warning text-dark', color: '#ffc107' },
    3: { nombre: 'Finalizado',  clase: 'bg-success',           color: '#198754' },
    4: { nombre: 'Descartado',  clase: 'bg-danger text-white', color: '#dc3545' }
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
   validarCapacidadEdicion() {
    const fermentador = this.fermentadores.find(f => f.id === Number(this.loteEditando.fermentadorId));
    
    if (fermentador && this.loteEditando.volumenLitros > fermentador.capacidad) {
      this.errorVolumenEdicion = `El volumen (${this.loteEditando.volumenLitros}L) supera la capacidad de ${fermentador.nombre} (${fermentador.capacidad}L).`;
      return false;
    }
    
    this.errorVolumenEdicion = null;
    return true;
  }

  abrirModal(p: any) {
    this.errorVolumenEdicion = null;
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
    if (!this.validarCapacidadEdicion()) return;
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
        Swal.fire({
        title: '¡Éxito!',
        text: 'Los cambios se guardaron correctamente.',
        icon: 'success',
        confirmButtonColor: '#8d4925'
      });
        this.cerrarModal();
        this.cargarPlanificaciones();
      },
      error: (e) => {
          console.error('Error al guardar', e);
      this.guardando = false;

      // --- LÓGICA PARA EL MENSAJE AMENO DE STOCK ---
      let mensajeDetalle = "No se pudieron guardar los cambios.";
      
      if (e.error && e.error.message) {
        mensajeDetalle = e.error.message; // Aquí viene el "Stock insuficiente..." del Backend
      } else if (typeof e.error === 'string') {
        mensajeDetalle = e.error;
      }

      Swal.fire({
        title: 'Atención con el Stock',
        text: mensajeDetalle,
        icon: 'warning', // Ícono amarillo de advertencia, menos agresivo que el rojo
        confirmButtonText: 'Entendido',
        confirmButtonColor: '#8d4925', // Tu color marrón
        background: '#ffffff'
      });
  
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
  eliminarLote(id: number) {
  Swal.fire({
    title: '¿Eliminar producción?',
    text: "Esta acción borrará el lote y liberará el fermentador permanentemente.",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#d33',
    cancelButtonColor: '#3085d6',
    confirmButtonText: 'Sí, eliminar',
    cancelButtonText: 'No, mantener'
  }).then((result) => {
    if (result.isConfirmed) {
      // Llamamos al servicio (asegurate que el nombre coincida con tu PlanificacionService)
      this._planifService.eliminar(id).subscribe({
        next: () => {
          Swal.fire('¡Eliminado!', 'El registro fue borrado con éxito.', 'success');
          this.cargarPlanificaciones(); // Refrescamos la lista
        },
        error: (err) => {
          console.error(err);
          Swal.fire('Error', 'No se pudo eliminar el lote.', 'error');
        }
      });
    }
  });
}
}