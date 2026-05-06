import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StockService, FormatoEnvaseDto, MovimientoDetalladoDto } from '../../services/stock.service';

@Component({
  selector: 'app-stock-gestion',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './stock-gestion.html',
  styleUrls: ['./stock-gestion.scss'],
})
export class StockGestion implements OnInit {
  tab: 'formatos' | 'stock' = 'formatos';
  expandedId: number | null = null;
  searchTerm = '';

  formatos: FormatoEnvaseDto[] = [];
  movimientos: MovimientoDetalladoDto[] = [];

  // Modal crear formato
  modalFormatoOpen = false;
  nuevoNombre = '';
  nuevoCapacidad: number | null = null;
  unidadCapacidad: 'L' | 'ml' = 'L';
  creandoFormato = false;
  errorFormato = '';

  // Modal eliminar
  deleteModalOpen = false;
  formatoAEliminar: FormatoEnvaseDto | null = null;

  constructor(private stockService: StockService) {}

  ngOnInit() {
    this.cargarFormatos();
    this.cargarMovimientos();
  }

  cargarFormatos() {
    this.stockService.getFormatosEnvase().subscribe({
      next: (data) => (this.formatos = data),
      error: () => console.error('Error cargando formatos'),
    });
  }

  cargarMovimientos() {
    this.stockService.getMovimientos().subscribe({
      next: (data) => (this.movimientos = data),
      error: () => {},
    });
  }

  get formatosFiltrados(): FormatoEnvaseDto[] {
    if (!this.searchTerm.trim()) return this.formatos;
    const term = this.searchTerm.toLowerCase();
    return this.formatos.filter(
      (f) =>
        f.nombre.toLowerCase().includes(term) ||
        f.productos.some((p) => p.estilo.toLowerCase().includes(term)),
    );
  }

  toggleExpand(id: number) {
    this.expandedId = this.expandedId === id ? null : id;
  }

  getTotalStock(formato: FormatoEnvaseDto): number {
    return formato.productos.reduce((sum, p) => sum + p.stockActual, 0);
  }

  // ── Crear Formato ─────────────────────────────────────────────────────

  openCrearFormato() {
    this.nuevoNombre = '';
    this.nuevoCapacidad = null;
    this.unidadCapacidad = 'L';
    this.errorFormato = '';
    this.modalFormatoOpen = true;
  }

  fmtCapacidad(litros: number): string {
    return litros < 1 ? `${litros * 1000} ml` : `${litros} L`;
  }

  guardarFormato() {
    if (!this.nuevoNombre.trim() || !this.nuevoCapacidad || this.nuevoCapacidad <= 0) {
      this.errorFormato = 'Completá nombre y capacidad válida';
      return;
    }
    const capacidadEnLitros = this.unidadCapacidad === 'ml'
      ? this.nuevoCapacidad / 1000
      : this.nuevoCapacidad;
    this.creandoFormato = true;
    this.errorFormato = '';
    this.stockService
      .crearFormatoEnvase({ nombre: this.nuevoNombre.trim(), capacidadLitros: capacidadEnLitros })
      .subscribe({
        next: () => {
          this.modalFormatoOpen = false;
          this.creandoFormato = false;
          this.cargarFormatos();
        },
        error: (err) => {
          this.errorFormato = err.error?.mensaje || 'Error al crear formato';
          this.creandoFormato = false;
        },
      });
  }

  // ── Eliminar Formato ──────────────────────────────────────────────────

  confirmarEliminar(formato: FormatoEnvaseDto) {
    this.formatoAEliminar = formato;
    this.deleteModalOpen = true;
  }

  eliminarFormato() {
    if (!this.formatoAEliminar) return;
    this.stockService.eliminarFormatoEnvase(this.formatoAEliminar.id).subscribe({
      next: () => {
        this.deleteModalOpen = false;
        this.formatoAEliminar = null;
        this.cargarFormatos();
      },
      error: () => alert('Error al eliminar formato'),
    });
  }
}
