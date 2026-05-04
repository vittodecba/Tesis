import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Lote } from '../../Interfaces/lote';
import { LoteService } from '../../services/lote';
import { FermentadorService } from '../../services/fermentador';
import { ArrowLeft, Edit2, Check, X, LucideAngularModule } from 'lucide-angular';
import { AuthService } from '../../core/services/auth.service';
import { RecetaService, Receta } from '../../services/receta';
import { StockService, FormatoEnvaseDto, LoteDesignacionDto } from '../../services/stock.service';

@Component({
  selector: 'app-lote-detalle',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './lote-detalle.html'
})
export class LoteDetalleComponent implements OnInit {
  lote: any | null = null;
  insumos: any[] = [];
  cargando: boolean = true;
  cargandoInsumos: boolean = false;
  error: string | null = null;

  fermentadoresDisponibles: any[] = [];
  editandoFermentador: boolean = false;
  nuevoFermentadorId: number | string = '';
  guardandoFermentador: boolean = false;
  usuarioActual: any = null;
  recetas: Receta[] = [];

  ArrowLeft = ArrowLeft;
  Edit2 = Edit2;
  Check = Check;
  X = X;

  estadosMapping: { [key: number]: { nombre: string, color: string } } = {
    1: { nombre: 'Planificado', color: 'bg-blue-100 text-blue-800 border-blue-200' },
    2: { nombre: 'En Proceso',  color: 'bg-yellow-100 text-yellow-800 border-yellow-200' },
    3: { nombre: 'Finalizado',  color: 'bg-green-100 text-green-800 border-green-200' },
    4: { nombre: 'Descartado',  color: 'bg-red-100 text-red-800 border-red-200' }
  };

  // ── Designaciones ─────────────────────────────────────────────────────
  designaciones: LoteDesignacionDto[] = [];
  formatosDisponibles: FormatoEnvaseDto[] = [];
  cargandoDesignaciones = false;

  nuevaDesignacionFormatoId: number | null = null;
  nuevaDesignacionVolumen: number | null = null;
  errorDesignacion = '';
  guardandoDesignacion = false;

  // ── Edición inline ────────────────────────────────────────────────────
  editandoDesignacionId: number | null = null;
  volumenEditado: number | null = null;
  guardandoEdicion = false;
  errorEdicion = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private loteService: LoteService,
    private fermentadorService: FermentadorService,
    private authService: AuthService,
    private recetaService: RecetaService,
    private stockService: StockService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      const idNumerico = Number(id);
      this.cargarLote(idNumerico)
      this.usuarioActual = this.authService.getCurrentUser();
      this.recetaService.getAll().subscribe({
      next: (data) => this.recetas = data ?? []
      });
    } else {
      this.error = 'No se proporcionó un ID de lote válido.';
      this.cargando = false;
    }
  }

  get loteRealId(): number {
    return this.lote?.loteId || this.lote?.id || 0;
  }

  cargarLote(id: number) {
    this.loteService.getLoteById(id).subscribe({
      next: (data) => {
        this.lote = data;
        this.cargando = false;
        this.cargarInsumos(data.id);
        this.cargarDesignaciones(data.loteId || data.id);
      },
      error: () => {
        this.error = 'Error al cargar el lote.';
        this.cargando = false;
      }
    });
  }

  cargarInsumos(id: number) {
    this.cargandoInsumos = true;
    this.loteService.getInsumosCombinados(id).subscribe({
      next: (data) => {
        this.insumos = data;
        this.cargandoInsumos = false;
      },
      error: () => { this.cargandoInsumos = false; }
    });
  }

  cargarFermentadores() {
    this.fermentadorService.getFermentadores().subscribe({
      next: (data) => {
        this.fermentadoresDisponibles = (data ?? []).filter((f: any) =>
          String(f.estado) === '1' || f.id === this.lote?.fermentadorId
        );
      },
      error: (err) => console.error('Error al cargar fermentadores', err)
    });
  }

  iniciarEdicionFermentador() {
    this.editandoFermentador = true;
    this.nuevoFermentadorId = '';
    this.cargarFermentadores();
  }

  cancelarEdicionFermentador() {
    this.editandoFermentador = false;
  }

  guardarFermentador() {
    if (!this.nuevoFermentadorId || !this.lote) return;
    this.guardandoFermentador = true;
    this.loteService.asignarFermentador(this.lote.id, Number(this.nuevoFermentadorId)).subscribe({
      next: () => {
        this.cargarLote(this.lote!.id);
        this.editandoFermentador = false;
        this.guardandoFermentador = false;
      },
      error: () => {
        alert('Error al vincular fermentador.');
        this.editandoFermentador = false;
        this.guardandoFermentador = false;
      }
    });
  }

  volver() {
    this.router.navigate(['/planificacion/Listado']);
  }
  
  getNombreReceta(recetaId: number): string {
    const receta = this.recetas.find(r => r.idReceta === recetaId);
    return receta ? receta.nombre : `Receta #${recetaId}`;
  }

  // ── Designaciones ─────────────────────────────────────────────────────

  get puedeDesignar(): boolean {
    const estado = this.lote?.estado;
    return estado === 'Planificado' || estado === 'EnProceso' ||
           estado === 'Planificado' || estado === 1 || estado === 2;
  }

  get volumenDesignado(): number {
    return this.designaciones.reduce((sum, d) => sum + d.volumenAsignado, 0);
  }

  get volumenRestante(): number {
    return (this.lote?.volumenLitros ?? 0) - this.volumenDesignado;
  }

  cargarDesignaciones(loteId: number) {
    this.cargandoDesignaciones = true;
    this.stockService.getDesignacionesByLote(loteId).subscribe({
      next: (data) => {
        this.designaciones = data;
        this.cargandoDesignaciones = false;
      },
      error: () => { this.cargandoDesignaciones = false; }
    });
    this.stockService.getFormatosEnvase().subscribe({
      next: (data) => (this.formatosDisponibles = data),
      error: () => {}
    });
  }

  agregarDesignacion() {
    if (!this.nuevaDesignacionFormatoId || !this.nuevaDesignacionVolumen || this.nuevaDesignacionVolumen <= 0) {
      this.errorDesignacion = 'Seleccioná un formato e ingresá un volumen válido';
      return;
    }
    this.guardandoDesignacion = true;
    this.errorDesignacion = '';
    this.stockService.addDesignacion(this.loteRealId, {
      formatoEnvaseId: this.nuevaDesignacionFormatoId,
      volumenAsignado: this.nuevaDesignacionVolumen
    }).subscribe({
      next: () => {
        this.nuevaDesignacionFormatoId = null;
        this.nuevaDesignacionVolumen = null;
        this.guardandoDesignacion = false;
        this.cargarDesignaciones(this.loteRealId);
      },
      error: (err) => {
        this.errorDesignacion = err.error?.mensaje || 'Error al agregar designación';
        this.guardandoDesignacion = false;
      }
    });
  }

  eliminarDesignacion(desId: number) {
    this.stockService.deleteDesignacion(this.loteRealId, desId).subscribe({
      next: () => this.cargarDesignaciones(this.loteRealId),
      error: () => alert('Error al eliminar designación')
    });
  }

  iniciarEdicion(des: LoteDesignacionDto) {
    this.editandoDesignacionId = des.id;
    this.volumenEditado = des.volumenAsignado;
    this.errorEdicion = '';
  }

  cancelarEdicion() {
    this.editandoDesignacionId = null;
    this.volumenEditado = null;
    this.errorEdicion = '';
  }

  guardarEdicion(des: LoteDesignacionDto) {
    if (!this.volumenEditado || this.volumenEditado <= 0) return;
    this.guardandoEdicion = true;
    this.errorEdicion = '';
    this.stockService.deleteDesignacion(this.loteRealId, des.id).subscribe({
      next: () => {
        this.stockService.addDesignacion(this.loteRealId, {
          formatoEnvaseId: des.formatoEnvaseId,
          volumenAsignado: this.volumenEditado!
        }).subscribe({
          next: () => {
            this.guardandoEdicion = false;
            this.cancelarEdicion();
            this.cargarDesignaciones(this.loteRealId);
          },
          error: (err) => {
            this.errorEdicion = err.error?.mensaje || 'Volumen inválido';
            this.guardandoEdicion = false;
            this.cargarDesignaciones(this.loteRealId);
          }
        });
      },
      error: () => {
        this.errorEdicion = 'Error al eliminar la designación anterior';
        this.guardandoEdicion = false;
      }
    });
  }
}