import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

type Formato = 'Barril' | 'Lata';
type Estado = 'Completado' | 'En proceso' | 'Pendiente';

interface EnvasadoRecienteUi {
  fecha: string; // 17/10/2024
  lote: string; // LT-2024-089
  formato: Formato; // Barril/Lata
  capacidad: string; // 50L / 733ml
  cantidad: number; // 8 / 480
  unidad: 'unidades'; // texto
  estado: Estado;
}

interface StockFormatoUi {
  formato: Formato;
  porcentaje: number; // 73.2
  litros: number; // 2400
}

interface StockEstiloUi {
  estilo: string; // IPA
  litros: number; // 1240
}

@Component({
  selector: 'app-stock-gestion',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './stock-gestion.html',
  styleUrl: './stock-gestion.scss',
})
export class StockGestion implements OnInit {
  // toolbar
  filtroFormato: 'Todos los formatos' | Formato = 'Todos los formatos';
  q = '';

  // data demo
  envasados: EnvasadoRecienteUi[] = [];
  stockFormato: StockFormatoUi[] = [];
  stockEstilo: StockEstiloUi[] = [];

  // escalas para barras
  maxLitrosEstilo = 0;

  ngOnInit(): void {
    this.envasados = [
      {
        fecha: '17/10/2024',
        lote: 'LT-2024-089',
        formato: 'Barril',
        capacidad: '50L',
        cantidad: 8,
        unidad: 'unidades',
        estado: 'Completado',
      },
      {
        fecha: '17/10/2024',
        lote: 'LT-2024-088',
        formato: 'Lata',
        capacidad: '733ml',
        cantidad: 480,
        unidad: 'unidades',
        estado: 'Completado',
      },
      {
        fecha: '16/10/2024',
        lote: 'LT-2024-087',
        formato: 'Barril',
        capacidad: '20L',
        cantidad: 12,
        unidad: 'unidades',
        estado: 'En proceso',
      },
      {
        fecha: '16/10/2024',
        lote: 'LT-2024-086',
        formato: 'Lata',
        capacidad: '500ml',
        cantidad: 360,
        unidad: 'unidades',
        estado: 'Completado',
      },
    ];

    this.stockFormato = [
      { formato: 'Barril', porcentaje: 73.2, litros: 2400 },
      { formato: 'Lata', porcentaje: 26.8, litros: 880 },
    ];

    this.stockEstilo = [
      { estilo: 'IPA', litros: 1240 },
      { estilo: 'Lager', litros: 980 },
      { estilo: 'Stout', litros: 760 },
      { estilo: 'Wheat', litros: 300 },
    ];

    this.maxLitrosEstilo = Math.max(...this.stockEstilo.map((x) => x.litros), 1);
  }

  // filtros
  get envasadosFiltrados(): EnvasadoRecienteUi[] {
    const q = this.q.trim().toLowerCase();

    return this.envasados.filter((e) => {
      const matchQ = !q
        ? true
        : e.lote.toLowerCase().includes(q) ||
          e.formato.toLowerCase().includes(q) ||
          e.capacidad.toLowerCase().includes(q) ||
          e.fecha.toLowerCase().includes(q);

      const matchFormato =
        this.filtroFormato === 'Todos los formatos' ? true : e.formato === this.filtroFormato;

      return matchQ && matchFormato;
    });
  }

  // badges
  estadoClass(estado: Estado): string {
    if (estado === 'Completado') return 'badge badge-ok';
    if (estado === 'En proceso') return 'badge badge-warn';
    return 'badge badge-neutral';
  }

  // “pastillas” de lote con color (como la captura)
  loteClass(formato: Formato): string {
    return formato === 'Barril' ? 'pill pill-blue' : 'pill pill-green';
  }

  // estilo -> color de barra
  estiloBarClass(estilo: string): string {
    const e = estilo.toLowerCase();
    if (e.includes('ipa')) return 'bar bar-orange';
    if (e.includes('lager')) return 'bar bar-blue';
    if (e.includes('stout')) return 'bar bar-green';
    return 'bar bar-purple';
  }
}
