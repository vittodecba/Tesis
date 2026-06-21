import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  BarrilDto,
  BarrilService,
  ClienteSimpleDto,
  CreateBarrilDto,
  FormatoRetornableDto,
} from '../../services/barril.service';
import { LucideAngularModule, Plus, Pencil, Barrel, Trash2, FileText, ChevronLeft, ChevronRight } from 'lucide-angular';
import { RouterLink } from '@angular/router';

interface OpcionEstado { valor: number; texto: string; }

@Component({
  selector: 'app-barriles-gestion',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, RouterLink], 
  templateUrl: './barriles-gestion.html',
})
export class BarrilesGestion implements OnInit {
  readonly Plus   = Plus;
  readonly Pencil = Pencil;
  readonly Barrel = Barrel;
  readonly Trash2 = Trash2;
  readonly Math   = Math;
  readonly FileText = FileText;
  readonly ChevronLeft = ChevronLeft;
  readonly ChevronRight = ChevronRight;

  lista: BarrilDto[] = [];
  filtrados: BarrilDto[] = [];
  formatos: FormatoRetornableDto[] = [];
  clientes: ClienteSimpleDto[] = [];

  filtroCodigo   = '';
  filtroEstado   = 'Todos';
  filtroFormato  = 0;
  filtroCliente  = 0;

  paginaActual = 1;
  readonly itemsPorPagina = 10;

  mostrarModalCrear  = false;
  mostrarModalEditar = false;
  guardando          = false;
  error              = '';

  nuevoBarril: CreateBarrilDto = this.crearVacio();
  barrilEditando: BarrilDto | null = null;
  observacionesEditar    = '';
  fechaAdquisicionEditar = '';
  estadoEditar: number | null = null;
  clienteIdEditar: number | null = null;

  private readonly transiciones: Record<number, OpcionEstado[]> = {
    0: [{ valor: 5, texto: 'Mantenimiento' }],
    1: [{ valor: 2, texto: 'Con Cliente' }],
    2: [{ valor: 3, texto: 'Sucio' }],
    3: [{ valor: 4, texto: 'En Lavado' }],
    4: [{ valor: 0, texto: 'Disponible' }],
    5: [{ valor: 0, texto: 'Disponible' }],
  };

  constructor(private _service: BarrilService) {}

  ngOnInit(): void {
    this.cargar();
    this._service.getFormatosRetornables().subscribe({
      next: (data) => (this.formatos = data.filter((f) => f.esRetornable)),
      error: () => {},
    });
    this._service.getClientes().subscribe({
      next: (data) => (this.clientes = data),
      error: () => {},
    });
  }

  cargar() {
    this._service.getBarriles().subscribe({
      next: (data) => { this.lista = data; this.aplicarFiltros(); },
      error: (err) => console.error(err),
    });
  }

  aplicarFiltros() {
    let res = [...this.lista];
    if (this.filtroCodigo.trim()) {
      const q = this.filtroCodigo.trim().toLowerCase();
      res = res.filter((b) => b.codigo.toLowerCase().includes(q));
    }
    if (this.filtroEstado !== 'Todos')
      res = res.filter((b) => b.estadoTexto === this.filtroEstado);
    if (this.filtroFormato > 0)
      res = res.filter((b) => b.formatoEnvaseId === this.filtroFormato);
    if (this.filtroCliente > 0)
      res = res.filter((b) => b.clienteId === this.filtroCliente);
    this.filtrados = res;
    this.paginaActual = 1;
  }

  get paginados(): BarrilDto[] {
    const inicio = (this.paginaActual - 1) * this.itemsPorPagina;
    return this.filtrados.slice(inicio, inicio + this.itemsPorPagina);
  }

  abrirCrear() {
    this.nuevoBarril = this.crearVacio();
    this.error = '';
    this.mostrarModalCrear = true;
  }

  cerrarCrear() { this.mostrarModalCrear = false; }

  guardarNuevo() {
    if (!this.nuevoBarril.codigo.trim()) { this.error = 'El código es obligatorio.'; return; }
    if (!this.nuevoBarril.formatoEnvaseId) { this.error = 'Debe seleccionar un formato.'; return; }
    this.guardando = true;
    this.error = '';
    this._service.crearBarril(this.nuevoBarril).subscribe({
      next: () => { this.guardando = false; this.cerrarCrear(); this.cargar(); },
      error: (err) => {
        this.guardando = false;
        this.error = typeof err.error === 'string' ? err.error : 'No se pudo crear el barril.';
      },
    });
  }

  abrirEditar(barril: BarrilDto) {
    this.barrilEditando         = barril;
    this.observacionesEditar    = barril.observaciones ?? '';
    this.fechaAdquisicionEditar = barril.fechaAdquisicion.substring(0, 10);
    this.estadoEditar           = null;
    this.clienteIdEditar        = barril.clienteId;
    this.error                  = '';
    this.mostrarModalEditar     = true;
  }

  cerrarEditar() { this.mostrarModalEditar = false; this.barrilEditando = null; }

  eliminar(barril: BarrilDto) {
    if (!confirm(`¿Seguro que querés eliminar el barril "${barril.codigo}"? Esta acción no se puede deshacer.`)) return;
    this._service.eliminarBarril(barril.id).subscribe({
      next: () => this.cargar(),
      error: (err) => alert(typeof err.error === 'string' ? err.error : 'No se pudo eliminar el barril.'),
    });
  }

  get opcionesEstado(): OpcionEstado[] {
    if (!this.barrilEditando) return [];
    return this.transiciones[this.barrilEditando.estado] ?? [];
  }

  guardarEdicion() {
    if (!this.barrilEditando) return;
    this.guardando = true;
    this.error = '';

    const clienteOriginal = this.barrilEditando.clienteId;
    const desasociar = clienteOriginal !== null && this.clienteIdEditar === null;
    const nuevoCliente = this.clienteIdEditar !== clienteOriginal && this.clienteIdEditar !== null
      ? this.clienteIdEditar : undefined;

    this._service
      .actualizarBarril(this.barrilEditando.id, {
        estado: this.estadoEditar ?? undefined,
        clienteId: nuevoCliente,
        desasociarCliente: desasociar,
        fechaAdquisicion: this.fechaAdquisicionEditar || undefined,
        observaciones: this.observacionesEditar || null,
      })
      .subscribe({
        next: () => { this.guardando = false; this.cerrarEditar(); this.cargar(); },
        error: (err) => {
          this.guardando = false;
          this.error = typeof err.error === 'string' ? err.error : 'No se pudo actualizar el barril.';
        },
      });
  }

  claseEstado(estadoTexto: string): string {
    const mapa: Record<string, string> = {
      'Disponible':    'bg-green-100 text-green-700',
      'Lleno':         'bg-blue-100 text-blue-700',
      'Con Cliente':   'bg-purple-100 text-purple-700',
      'Sucio':         'bg-yellow-100 text-yellow-700',
      'En Lavado':     'bg-orange-100 text-orange-700',
      'Mantenimiento': 'bg-red-100 text-red-700',
    };
    return mapa[estadoTexto] ?? 'bg-gray-100 text-gray-700';
  }

  formatearCapacidad(litros: number): string {
    return litros < 1 ? `${litros * 1000}ml` : `${litros}L`;
  }

  private crearVacio(): CreateBarrilDto {
    const hoy = new Date().toISOString().substring(0, 10);
    return { codigo: '', formatoEnvaseId: 0, fechaAdquisicion: hoy, observaciones: null };
  }
}