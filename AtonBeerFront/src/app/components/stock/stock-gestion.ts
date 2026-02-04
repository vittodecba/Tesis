import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

type Tone = 'ok' | 'warn' | 'danger';
type Tab = 'stock' | 'movimientos';

interface EstiloStock {
  id: string;
  nombre: string;
  stock: number;
  min: number;
  ultima: string;
}

interface ProductoStock {
  id: string;
  nombre: string;
  unidad: string;
  estilos: EstiloStock[];
}

interface MovimientoStock {
  id: string;
  fecha: string;
  producto: string;
  estilo: string;
  tipo: 'Entrada' | 'Salida' | 'Venta' | 'Ajuste';
  cantidad: number;
  usuario: string;
  nota: string;
}

@Component({
  selector: 'app-stock-gestion',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './stock-gestion.html',
  styleUrls: ['./stock-gestion.scss'],
})
export class StockGestion {
  Math = Math;

  // Paleta (igual que tu referencia)
  COLORS = {
    brown: '#4A2C2A',
    orange: '#E67E22',
    orangeDark: '#D35400',
  };

  tab: Tab = 'stock';
  expandedId: string = 'keg-10';

  // Modal
  modalOpen = false;
  formProductoNombre = '';
  formProductoUnidad: 'u.' | 'cajas' | 'packs' = 'u.';

  // Filtros movimientos
  qMov = '';
  tipoMov: 'all' | MovimientoStock['tipo'] = 'all';

  products: ProductoStock[] = [
    {
      id: 'keg-10',
      nombre: 'Barril 10L',
      unidad: 'u.',
      estilos: [
        { id: 'ipa-10', nombre: 'IPA', stock: 12, min: 6, ultima: 'Entrada +4 (Hoy)' },
        { id: 'golden-10', nombre: 'Golden', stock: 4, min: 6, ultima: 'Salida -2 (Ayer)' },
        { id: 'stout-10', nombre: 'Stout', stock: 8, min: 6, ultima: 'Entrada +2 (Hoy)' },
      ],
    },
    {
      id: 'keg-20',
      nombre: 'Barril 20L',
      unidad: 'u.',
      estilos: [
        { id: 'ipa-20', nombre: 'IPA', stock: 6, min: 4, ultima: 'Salida -1 (Hoy)' },
        { id: 'golden-20', nombre: 'Golden', stock: 2, min: 4, ultima: 'Salida -1 (Ayer)' },
        { id: 'amber-20', nombre: 'Amber', stock: 5, min: 4, ultima: 'Entrada +5 (Hace 2d)' },
      ],
    },
    {
      id: 'keg-50',
      nombre: 'Barril 50L',
      unidad: 'u.',
      estilos: [
        { id: 'ipa-50', nombre: 'IPA', stock: 2, min: 3, ultima: 'Salida -1 (Ayer)' },
        { id: 'stout-50', nombre: 'Stout', stock: 4, min: 3, ultima: 'Entrada +1 (Hoy)' },
      ],
    },
    {
      id: 'can-473',
      nombre: 'Latas 473ml',
      unidad: 'u.',
      estilos: [
        { id: 'golden-473', nombre: 'Golden', stock: 320, min: 120, ultima: 'Venta -24 (Hoy)' },
        { id: 'ipa-473', nombre: 'IPA', stock: 140, min: 120, ultima: 'Producción +48 (Ayer)' },
        { id: 'porter-473', nombre: 'Porter', stock: 80, min: 120, ultima: 'Venta -12 (Hoy)' },
      ],
    },
  ];

  movements: MovimientoStock[] = [
    {
      id: 'm1',
      fecha: '2026-02-02 10:15',
      producto: 'Barril 20L',
      estilo: 'IPA',
      tipo: 'Salida',
      cantidad: -1,
      usuario: 'Sistema',
      nota: 'Despacho a franquicia',
    },
    {
      id: 'm2',
      fecha: '2026-02-02 09:40',
      producto: 'Barril 10L',
      estilo: 'IPA',
      tipo: 'Entrada',
      cantidad: 4,
      usuario: 'Juli',
      nota: 'Envasado',
    },
    {
      id: 'm3',
      fecha: '2026-02-01 18:05',
      producto: 'Latas 473ml',
      estilo: 'Golden',
      tipo: 'Venta',
      cantidad: -24,
      usuario: 'Sistema',
      nota: 'Venta e-commerce',
    },
    {
      id: 'm4',
      fecha: '2026-02-01 12:30',
      producto: 'Barril 50L',
      estilo: 'Stout',
      tipo: 'Entrada',
      cantidad: 1,
      usuario: 'Admin',
      nota: 'Transferencia desde maduración',
    },
    {
      id: 'm5',
      fecha: '2026-01-31 20:10',
      producto: 'Latas 473ml',
      estilo: 'Porter',
      tipo: 'Venta',
      cantidad: -12,
      usuario: 'Sistema',
      nota: 'Venta mostrador',
    },
  ];

  // ---------- UI ----------
  setTab(t: Tab) {
    this.tab = t;
  }

  isExpanded(id: string): boolean {
    return this.expandedId === id;
  }

  toggleExpand(id: string) {
    this.expandedId = this.expandedId === id ? '' : id;
  }

  // ---------- Totales / status ----------
  totalProducto(p: ProductoStock): number {
    return p.estilos.reduce((acc, e) => acc + (e.stock ?? 0), 0);
  }

  minSumProducto(p: ProductoStock): number {
    return p.estilos.reduce((acc, e) => acc + (e.min ?? 0), 0);
  }

  totalAll(): number {
    return this.products.reduce((acc, p) => acc + this.totalProducto(p), 0);
  }

  criticalCount(): number {
    let c = 0;
    for (const p of this.products) {
      const total = this.totalProducto(p);
      const minSum = this.minSumProducto(p);
      const s = this.statusFor(total, Math.max(1, minSum / 2));
      if (s.tone === 'danger') c++;
    }
    return c;
  }

  statusFor(stock: number, min: number): { label: string; tone: Tone } {
    if (stock <= 0) return { label: 'Sin stock', tone: 'danger' };
    if (stock < min) return { label: 'Crítico', tone: 'danger' };
    if (stock < min * 1.5) return { label: 'Bajo', tone: 'warn' };
    return { label: 'OK', tone: 'ok' };
  }

  toneChipClass(tone: Tone): string {
    if (tone === 'danger') return 'bg-red-100 text-red-700';
    if (tone === 'warn') return 'bg-amber-100 text-amber-800';
    return 'bg-green-100 text-green-700';
  }

  toneDotClass(tone: Tone): string {
    if (tone === 'danger') return 'bg-red-500';
    if (tone === 'warn') return 'bg-amber-500';
    return 'bg-green-500';
  }

  toneBarClass(tone: Tone): string {
    if (tone === 'danger') return 'bg-red-500';
    if (tone === 'warn') return 'bg-amber-500';
    return 'bg-green-500';
  }

  progressPct(p: ProductoStock): number {
    const total = this.totalProducto(p);
    const minSum = this.minSumProducto(p);
    const denom = Math.max(1, minSum);
    return Math.min(100, (total / denom) * 100);
  }

  // ---------- Movimientos ----------
  filteredMovements(): MovimientoStock[] {
    const q = (this.qMov ?? '').trim().toLowerCase();
    return this.movements.filter((m) => {
      const blob =
        `${m.fecha} ${m.producto} ${m.estilo} ${m.tipo} ${m.usuario} ${m.nota}`.toLowerCase();
      const matchQ = q ? blob.includes(q) : true;
      const matchTipo = this.tipoMov === 'all' ? true : m.tipo === this.tipoMov;
      return matchQ && matchTipo;
    });
  }

  // ---------- Modal ----------
  openModal() {
    this.modalOpen = true;
    this.formProductoNombre = '';
    this.formProductoUnidad = 'u.';
  }

  closeModal() {
    this.modalOpen = false;
  }

  saveProducto() {
    const clean = (this.formProductoNombre ?? '').trim();
    if (!clean) return;

    const id = clean.toLowerCase().replace(/\s+/g, '-') + '-' + Date.now();

    this.products = [
      ...this.products,
      {
        id,
        nombre: clean,
        unidad: this.formProductoUnidad,
        estilos: [],
      },
    ];

    this.modalOpen = false;
  }

  // ---------- Helpers ----------
  isCan(p: ProductoStock): boolean {
    return p.nombre.toLowerCase().includes('lata');
  }
}
