import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StockService } from '../../services/stock.service';

type Tone = 'ok' | 'warn' | 'danger';

interface EstiloStock {
  id: string;
  nombre: string;
  stock: number;
  min: number;
}

interface ProductoStock {
  id: string;
  nombre: string;
  unidad: string;
  estilos: EstiloStock[];
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
  products: ProductoStock[] = [];
  movements: any[] = [];

  // Filtros y Variables de UI (Esto faltaba)
  tipoMov: string = 'all';

  // Formulario
  modalOpen = false;
  formProductoNombre = '';
  formProductoEstilo = '';
  formProductoUnidad = 'Litro';

  constructor(private stockService: StockService) {}

  ngOnInit() {
    this.cargarDatos();
  }

  cargarDatos() {
    this.stockService.getProductos().subscribe({
      next: (data) => {
        this.products = this.agruparProductos(data);
      },
      error: (err) => console.error('Error cargando productos', err),
    });

    this.stockService.getMovimientos().subscribe((data) => (this.movements = data));
  }

  private agruparProductos(backendData: any[]): ProductoStock[] {
    const grupos: { [key: string]: ProductoStock } = {};
    backendData.forEach((p) => {
      const nombreGrupo = p.formato || 'Sin Formato';
      if (!grupos[nombreGrupo]) {
        grupos[nombreGrupo] = {
          id: nombreGrupo.toLowerCase().replace(/\s+/g, '-'),
          nombre: nombreGrupo,
          unidad: p.unidadMedida || 'u.',
          estilos: [],
        };
      }
      grupos[nombreGrupo].estilos.push({
        id: p.id.toString(),
        nombre: p.estilo || 'General',
        stock: p.stockActual || 0,
        min: 10,
      });
    });
    return Object.values(grupos);
  }

  // --- FUNCIONES QUE FALTABAN PARA EL HTML ---

  criticalCount(): number {
    return this.products.filter((p) => {
      const total = this.totalProducto(p);
      return total < this.minSumProducto(p) / 2;
    }).length;
  }

  isCan(p: ProductoStock): boolean {
    return p.nombre.toLowerCase().includes('lata');
  }

  filteredMovements() {
    if (this.tipoMov === 'all') return this.movements;
    return this.movements.filter((m) => m.tipoMovimiento === this.tipoMov);
  }

  // --- LÓGICA DE UI Y ESTILOS ---

  totalProducto(p: ProductoStock) {
    return p.estilos.reduce((acc, e) => acc + e.stock, 0);
  }
  minSumProducto(p: ProductoStock) {
    return p.estilos.reduce((acc, e) => acc + e.min, 0);
  }
  totalAll() {
    return this.products.reduce((acc, p) => acc + this.totalProducto(p), 0);
  }

  statusFor(stock: number, min: number): { label: string; tone: Tone } {
    if (stock <= 0) return { label: 'Sin stock', tone: 'danger' };
    if (stock < min) return { label: 'Crítico', tone: 'danger' };
    return { label: 'OK', tone: 'ok' };
  }

  toneChipClass(t: Tone) {
    return t === 'danger' ? 'bg-red-100 text-red-700' : 'bg-green-100 text-green-700';
  }
  toneDotClass(t: Tone) {
    return t === 'danger' ? 'bg-red-500' : 'bg-green-500';
  }
  toneBarClass(t: Tone) {
    return t === 'danger' ? 'bg-red-500' : 'bg-green-500';
  }

  progressPct(p: ProductoStock) {
    const total = this.totalProducto(p);
    return Math.min(100, (total / (this.minSumProducto(p) || 1)) * 100);
  }

  // --- ACCIONES ---

  toggleExpand(id: string) {
    this.expandedId = this.expandedId === id ? '' : id;
  }
  isExpanded(id: string) {
    return this.expandedId === id;
  }
  setTab(t: 'stock' | 'movimientos') {
    this.tab = t;
  }
  openModal() {
    this.modalOpen = true;
  }
  closeModal() {
    this.modalOpen = false;
  }

  saveProducto() {
    if (!this.formProductoNombre || !this.formProductoEstilo) return;
    const payload = {
      nombre: `${this.formProductoNombre} ${this.formProductoEstilo}`,
      estilo: this.formProductoEstilo,
      formato: this.formProductoNombre,
      unidadMedida: this.formProductoUnidad,
      stockActual: 0,
    };
    this.stockService.crearProducto(payload).subscribe(() => {
      this.cargarDatos();
      this.closeModal();
      this.formProductoNombre = '';
      this.formProductoEstilo = '';
    });
  }
}
