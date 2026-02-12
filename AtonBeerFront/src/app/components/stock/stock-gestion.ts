import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StockService } from '../../services/stock.service';

interface FormatoAgrupado {
  formato: string;
  unidadMedida: string;
  items: any[];
  totalStock: number;
}

@Component({
  selector: 'app-stock-gestion',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './stock-gestion.html',
  styleUrls: ['./stock-gestion.scss'],
})
export class StockGestion implements OnInit {
  COLORS = { brown: '#4A2C2A', orange: '#E67E22' };
  tab: 'stock' | 'movimientos' = 'stock';
  expandedId: string = '';
  searchTerm: string = ''; // <--- Para el buscador

  formatosAgrupados: FormatoAgrupado[] = [];
  movimientos: any[] = [];

  // Modales
  modalProdOpen = false;
  movModalOpen = false;
  deleteModalOpen = false;

  // Estado
  isEditing = false;
  editProductoId: number | null = null;
  prodAEliminar: any = null;

  // Formulario
  prodEstilo = '';
  prodFormato = '';
  movProductoSeleccionado: any = null;
  movCantidad: number | null = null;

  constructor(private stockService: StockService) {}

  ngOnInit() {
    this.cargarTodo();
  }

  cargarTodo() {
    this.stockService.getProductos().subscribe((productos: any[]) => {
      this.agruparProductos(productos);
    });
    this.stockService.getMovimientos().subscribe((movs: any[]) => {
      this.movimientos = movs;
    });
  }

  agruparProductos(productos: any[]) {
    const grupos: { [key: string]: FormatoAgrupado } = {};
    productos.forEach((p) => {
      const f = p.formato || p.Formato || 'Sin Formato';
      if (!grupos[f]) {
        grupos[f] = {
          formato: f,
          unidadMedida: p.unidadMedida || p.UnidadMedida || 'u.',
          items: [],
          totalStock: 0,
        };
      }
      grupos[f].items.push(p);
      grupos[f].totalStock += p.stockActual || p.StockActual || 0;
    });
    this.formatosAgrupados = Object.values(grupos);
  }

  // --- GETTER PARA EL FILTRO ---
  get gruposFiltrados() {
    if (!this.searchTerm.trim()) return this.formatosAgrupados;

    const term = this.searchTerm.toLowerCase();
    return this.formatosAgrupados
      .map((grupo) => {
        const itemsFiltrados = grupo.items.filter((item) =>
          (item.estilo || item.Estilo).toLowerCase().includes(term),
        );
        return { ...grupo, items: itemsFiltrados };
      })
      .filter((grupo) => grupo.items.length > 0);
  }

  toggleExpand(id: string) {
    this.expandedId = this.expandedId === id ? '' : id;
  }

  // --- ACCIONES ---
  openCreateModal() {
    this.isEditing = false;
    this.editProductoId = null;
    this.prodEstilo = '';
    this.prodFormato = '';
    this.modalProdOpen = true;
  }

  openEditModal(item: any) {
    this.isEditing = true;
    this.editProductoId = item.id || item.Id;
    this.prodEstilo = item.estilo || item.Estilo;
    this.prodFormato = item.formato || item.Formato;
    this.modalProdOpen = true;
  }

  guardarProducto() {
    if (!this.prodEstilo || !this.prodFormato) return;
    const dto = {
      Nombre: `${this.prodFormato} ${this.prodEstilo}`,
      Estilo: this.prodEstilo,
      Formato: this.prodFormato,
      UnidadMedida: 'Unidades',
    };

    if (this.isEditing && this.editProductoId) {
      this.stockService
        .actualizarProducto(this.editProductoId, dto)
        .subscribe(() => this.finalizar());
    } else {
      this.stockService.crearProducto(dto).subscribe(() => this.finalizar());
    }
  }

  confirmarEliminar(item: any) {
    this.prodAEliminar = item;
    this.deleteModalOpen = true;
  }

  eliminarProducto() {
    const id = this.prodAEliminar.id || this.prodAEliminar.Id;
    this.stockService.eliminarProducto(id).subscribe({
      next: () => {
        this.deleteModalOpen = false;
        this.cargarTodo();
      },
      error: () => alert('Error al eliminar producto.'),
    });
  }

  finalizar() {
    this.modalProdOpen = false;
    this.cargarTodo();
  }

  openMovModal(item: any) {
    this.movProductoSeleccionado = item;
    this.movCantidad = null;
    this.movModalOpen = true;
  }

  confirmarMovimiento() {
    if (!this.movProductoSeleccionado || !this.movCantidad) return;
    const dto = {
      productoId: this.movProductoSeleccionado.id || this.movProductoSeleccionado.Id,
      cantidad: Math.abs(this.movCantidad),
      tipoMovimiento: this.movCantidad > 0 ? 'Ingreso' : 'Egreso',
      motivoMovimiento: 'Ajuste manual',
      Fecha: new Date().toISOString(),
    };
    this.stockService.registrarMovimiento(dto).subscribe(() => {
      this.movModalOpen = false;
      this.cargarTodo();
    });
  }
}
