import Swal from 'sweetalert2';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StockService, FormatoEnvaseDto, MovimientoDetalladoDto, ProductoStockDto } from '../../services/stock.service';

interface GrupoEstilo {
  estilo: string;
  total: number;
  items: ProductoStockDto[];
}

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

  modalFormatoOpen = false;
  nuevoNombre = '';
  nuevoCapacidad: number | null = null;
  unidadCapacidad: 'L' | 'ml' = 'L';
  nuevoEsRetornable = false;
  creandoFormato = false;
  errorFormato = '';

  deleteModalOpen = false;
  formatoAEliminar: FormatoEnvaseDto | null = null;

  ingresoModalOpen = false;
  ingresoProducto: { id: number; estilo: string; formatoNombre: string } | null = null;
  ingresoCantidad: number | null = null;
  ingresoMotivo = 'Ingreso Manual';
  ingresando = false;
  errorIngreso = '';

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
        f.productos.some(
        (p) =>
          p.estilo.toLowerCase().includes(term) ||
          (p.recetaNombre?.toLowerCase().includes(term) ?? false),
      ),
    );
  }

  get movimientosFiltrados(): MovimientoDetalladoDto[] {
    if (!this.searchTerm.trim()) return this.movimientos;
    const term = this.searchTerm.toLowerCase();
    return this.movimientos.filter(
      (m) =>
        (m.estilo?.toLowerCase().includes(term) ?? false) ||
        (m.recetaNombre?.toLowerCase().includes(term) ?? false) ||
        (m.formatoNombre?.toLowerCase().includes(term) ?? false) ||
        (m.tipoMovimiento?.toLowerCase().includes(term) ?? false) ||
        (m.motivoMovimiento?.toLowerCase().includes(term) ?? false) ||
        (m.loteId != null && `l-${m.loteId}`.includes(term)),
    );
  }

  toggleExpand(id: number) {
    this.expandedId = this.expandedId === id ? null : id;
  }

  getTotalStock(formato: FormatoEnvaseDto): number {
    return formato.productos.reduce((sum, p) => sum + p.stockActual, 0);
  }

  agruparPorEstilo(productos: ProductoStockDto[]): GrupoEstilo[] {
    const mapa = new Map<string, ProductoStockDto[]>();
    for (const p of productos) {
      if (!mapa.has(p.estilo)) mapa.set(p.estilo, []);
      mapa.get(p.estilo)!.push(p);
    }
    return Array.from(mapa.entries()).map(([estilo, items]) => ({
      estilo,
      total: items.reduce((s, p) => s + p.stockActual, 0),
      items
    }));
  }

  esGrupoSimple(grupo: GrupoEstilo): boolean {
    return grupo.items.length === 1 && !grupo.items[0].recetaNombre;
  }
  // ── Crear Formato ─────────────────────────────────────────────────────

  openCrearFormato() {
    this.nuevoNombre = '';
    this.nuevoCapacidad = null;
    this.unidadCapacidad = 'L';
    this.nuevoEsRetornable = false;
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
      .crearFormatoEnvase({ 
        nombre: this.nuevoNombre.trim(), 
        capacidadLitros: capacidadEnLitros, 
        esRetornable: this.nuevoEsRetornable 
      })
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
      error: (err) => {
        this.deleteModalOpen = false;
        this.formatoAEliminar = null;
        Swal.fire('No se puede eliminar', err.error?.mensaje || 'Error al eliminar el formato.', 'warning');
      },
    });
  }

  correccionProducto: { id: number; estilo: string; formatoNombre: string; stockActual: number } | null = null;
  correccionNuevaCantidad: number | null = null;
  corrigiendo = false;
  errorCorreccion = '';

  abrirCorreccion(prod: ProductoStockDto, formatoNombre: string) {
    this.correccionProducto = { id: prod.id, estilo: prod.estilo, formatoNombre, stockActual: prod.stockActual };
    this.correccionNuevaCantidad = prod.stockActual;
    this.errorCorreccion = '';
    this.corrigiendo = false;
  }

  confirmarCorreccion() {
    if (!this.correccionProducto || this.correccionNuevaCantidad === null || this.correccionNuevaCantidad < 0) {
      this.errorCorreccion = 'Ingresá una cantidad válida (≥ 0).';
      return;
    }
    this.corrigiendo = true;
    this.errorCorreccion = '';
    this.stockService.corregirStock(this.correccionProducto.id, this.correccionNuevaCantidad).subscribe({
      next: () => {
        this.correccionProducto = null;
        this.corrigiendo = false;
        this.cargarFormatos();
        this.cargarMovimientos();
      },
      error: (err) => {
        this.errorCorreccion = err.error?.mensaje || 'Error al corregir stock.';
        this.corrigiendo = false;
      },
    });
  }

  egresoProducto: { id: number; estilo: string; formatoNombre: string; stockActual: number } | null = null;
  egresoCantidad: number | null = null;
  egresoMotivo = 'Egreso Manual';
  egresando = false;
  errorEgreso = '';

  abrirEgreso(prod: ProductoStockDto, formatoNombre: string) {
    this.egresoProducto = { id: prod.id, estilo: prod.estilo, formatoNombre, stockActual: prod.stockActual };
    this.egresoCantidad = null;
    this.egresoMotivo = 'Egreso Manual';
    this.errorEgreso = '';
    this.egresando = false;
  }

  confirmarEgreso() {
    if (!this.egresoProducto || !this.egresoCantidad || this.egresoCantidad <= 0) {
      this.errorEgreso = 'Ingresá una cantidad válida mayor a 0.';
      return;
    }
    this.egresando = true;
    this.errorEgreso = '';
    this.stockService.egresoManual({
      productoStockId: this.egresoProducto.id,
      cantidad: this.egresoCantidad,
      motivo: this.egresoMotivo || 'Egreso Manual',
    }).subscribe({
      next: () => {
        this.egresoProducto = null;
        this.egresando = false;
        this.cargarFormatos();
        this.cargarMovimientos();
      },
      error: (err) => {
        this.errorEgreso = err.error?.mensaje || 'Error al registrar egreso.';
        this.egresando = false;
      },
    });
  }

  abrirIngreso(prod: ProductoStockDto, formatoNombre: string) {
    this.ingresoProducto = { id: prod.id, estilo: prod.estilo, formatoNombre };
    this.ingresoCantidad = null;
    this.ingresoMotivo = 'Ingreso Manual';
    this.errorIngreso = '';
    this.ingresoModalOpen = true;
  }

  confirmarIngreso() {
    if (!this.ingresoProducto || !this.ingresoCantidad || this.ingresoCantidad <= 0) {
      this.errorIngreso = 'Ingresá una cantidad válida mayor a 0';
      return;
    }
    this.ingresando = true;
    this.errorIngreso = '';
    this.stockService.agregarIngresoManual({
      productoStockId: this.ingresoProducto.id,
      cantidad: this.ingresoCantidad,
      motivo: this.ingresoMotivo || 'Ingreso Manual',
    }).subscribe({
      next: () => {
        this.ingresoModalOpen = false;
        this.ingresando = false;
        this.cargarFormatos();
        this.cargarMovimientos();
      },
      error: (err) => {
        this.errorIngreso = err.error?.mensaje || 'Error al registrar el ingreso';
        this.ingresando = false;
      },
    });
  }
}