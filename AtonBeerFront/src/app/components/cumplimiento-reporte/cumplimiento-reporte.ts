import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { BaseChartDirective } from 'ng2-charts';
import { ChartOptions, Chart, registerables } from 'chart.js';
import {
  CumplimientoReporteService,
  ReporteCumplimiento,
  LoteCumplimiento,
} from '../../services/cumplimiento-reporte.service';

Chart.register(...registerables);

@Component({
  selector: 'app-cumplimiento-reporte',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, BaseChartDirective],
  templateUrl: './cumplimiento-reporte.html',
  styleUrl: './cumplimiento-reporte.css',
})
export class CumplimientoReporte implements OnInit {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  filtroForm: FormGroup;
  cargando = false;
  descargandoPdf = false;
  hayDatos = false;

  resumen = {
    lotesFinalizados: 0,
    porcentajeATiempo: 0,
    desvioPromedioDias: 0,
    tasaDescarte: 0,
    lotesExcluidos: 0,
  };

  detalle: LoteCumplimiento[] = [];
  // Subconjunto que se grafica: solo finalizados (los descartados no tienen desvío de cumplimiento).
  detalleGrafico: LoteCumplimiento[] = [];

  // Ancho mínimo del canvas: crece con la cantidad de lotes para que las barras no se
  // amontonen; el contenedor tiene scroll horizontal cuando supera el ancho visible.
  public chartMinWidth = 600;

  // Colores de serie validados (par azul/naranja, CVD-safe). Estimado = referencia (azul),
  // Real = valor de marca (naranja).
  private readonly COLOR_ESTIMADO = '#2a78d6';
  private readonly COLOR_REAL = '#E67E22';

  public barChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false,
    // 'index' → al pasar el mouse muestra AMBAS series (Estimado y Real) del mismo lote.
    interaction: { mode: 'index', intersect: false },
    plugins: {
      legend: {
        position: 'top',
        labels: {
          usePointStyle: true,
          pointStyle: 'circle',
          boxWidth: 8,
          boxHeight: 8,
          padding: 16,
          color: '#52514e',
          font: { size: 13, weight: 600 },
        },
      },
      tooltip: {
        backgroundColor: '#3A2220',
        titleColor: '#ffffff',
        bodyColor: '#f5f5f4',
        footerColor: '#fcd9b6',
        padding: 12,
        cornerRadius: 10,
        titleFont: { size: 13, weight: 700 },
        bodyFont: { size: 12 },
        footerFont: { size: 12, weight: 700 },
        usePointStyle: true,
        callbacks: {
          // Título: código del lote + estilo (que sacamos del eje X para no amontonar).
          title: (items: any) => {
            const l = this.detalleGrafico[items[0]?.dataIndex];
            return l ? `${l.codigoLote}${l.estilo ? ' · ' + l.estilo : ''}` : '';
          },
          label: (ctx: any) => `  ${ctx.dataset.label}: ${ctx.parsed.y} días`,
          // Pie: desvío del lote (real - estimado), con signo.
          footer: (items: any) => {
            const l = this.detalleGrafico[items[0]?.dataIndex];
            if (!l) return '';
            const pct = this.desvioPorcentaje(l);
            if (pct === null) return 'Desvío: —';
            const signo = pct > 0 ? '+' : '';
            return `Desvío: ${signo}${pct}%`;
          },
        },
      },
    },
    scales: {
      x: {
        grid: { display: false },
        border: { color: '#c3c2b7' },
        ticks: { color: '#898781', font: { size: 11 }, maxRotation: 55, minRotation: 45, autoSkip: false },
      },
      y: {
        beginAtZero: true,
        title: { display: true, text: 'Días', color: '#898781', font: { size: 12, weight: 600 } },
        grid: { color: '#e1e0d9' },
        border: { display: false },
        ticks: { color: '#898781', font: { size: 11 }, precision: 0 },
      },
    },
  };
  public barChartLabels: string[] = [];
  public barChartDatasets: any[] = [
    this.buildDataset([], 'Estimado', this.COLOR_ESTIMADO),
    this.buildDataset([], 'Real', this.COLOR_REAL),
  ];

  // Barras finas, con puntas redondeadas ancladas a la base y una separación de 2px
  // (borde del color de superficie) entre barras adyacentes.
  private buildDataset(data: number[], label: string, color: string) {
    return {
      data,
      label,
      backgroundColor: color,
      hoverBackgroundColor: color,
      borderColor: '#ffffff',
      borderWidth: 2,
      borderRadius: 4,
      borderSkipped: false,
      categoryPercentage: 0.7,
      barPercentage: 0.9,
      maxBarThickness: 30,
    };
  }

  // Desvío como % respecto de los días estimados: (real - estimado) / estimado * 100.
  // null cuando no hay estimado (evita dividir por cero).
  desvioPorcentaje(l: LoteCumplimiento): number | null {
    if (l.estado !== 'Finalizado') return null; // descartado: no completó fermentación, sin desvío de cumplimiento
    if (!l.diasEstimados) return null;
    return Math.round((l.desvioDias / l.diasEstimados) * 100);
  }

  // Promedio de los desvíos porcentuales, solo sobre lotes FINALIZADOS (igual que el
  // cálculo original en días del backend; los descartados no cuentan para el cumplimiento).
  get desvioPromedioPorcentaje(): number | null {
    const vals = this.detalle
      .filter((l) => l.estado === 'Finalizado')
      .map((l) => this.desvioPorcentaje(l))
      .filter((v): v is number => v !== null);
    if (!vals.length) return null;
    return Math.round(vals.reduce((a, b) => a + b, 0) / vals.length);
  }

  private fb = inject(FormBuilder);
  private reporteService = inject(CumplimientoReporteService);

  constructor() {
    this.filtroForm = this.fb.group({
      periodoRapido: ['ultimos_6_meses'],
      fechaDesde: [''],
      fechaHasta: [''],
    });
  }

  ngOnInit(): void {
    this.filtroForm.get('periodoRapido')?.valueChanges.subscribe((valor) => {
      const hoy = new Date();
      let desde = '';
      let hasta = '';

      if (valor === 'mes_actual') {
        desde = new Date(hoy.getFullYear(), hoy.getMonth(), 1).toISOString().split('T')[0];
        hasta = new Date(hoy.getFullYear(), hoy.getMonth() + 1, 0).toISOString().split('T')[0];
      } else if (valor === 'ultimos_3_meses') {
        desde = new Date(hoy.getFullYear(), hoy.getMonth() - 3, 1).toISOString().split('T')[0];
        hasta = new Date(hoy.getFullYear(), hoy.getMonth() + 1, 0).toISOString().split('T')[0];
      } else if (valor === 'ultimos_6_meses') {
        desde = new Date(hoy.getFullYear(), hoy.getMonth() - 6, 1).toISOString().split('T')[0];
        hasta = new Date(hoy.getFullYear(), hoy.getMonth() + 1, 0).toISOString().split('T')[0];
      } else if (valor === 'anio_actual') {
        desde = new Date(hoy.getFullYear(), 0, 1).toISOString().split('T')[0];
        hasta = new Date(hoy.getFullYear(), 11, 31).toISOString().split('T')[0];
      } else if (valor === 'anio_anterior') {
        desde = new Date(hoy.getFullYear() - 1, 0, 1).toISOString().split('T')[0];
        hasta = new Date(hoy.getFullYear() - 1, 11, 31).toISOString().split('T')[0];
      }

      if (desde && hasta) {
        this.filtroForm.patchValue({ fechaDesde: desde, fechaHasta: hasta }, { emitEvent: false });
      }
    });

    // Dispara el cálculo de fechas para el período por defecto (Últimos 6 meses).
    this.filtroForm.get('periodoRapido')?.updateValueAndValidity();
  }

  filtrarReporte(): void {
    const { fechaDesde, fechaHasta } = this.filtroForm.value;
    if (!fechaDesde || !fechaHasta) return;

    this.cargando = true;

    this.reporteService.obtenerReporte(fechaDesde, fechaHasta).subscribe({
      next: (data: ReporteCumplimiento) => {
        this.resumen = {
          lotesFinalizados: Number(data.lotesFinalizados) || 0,
          porcentajeATiempo: Number(data.porcentajeATiempo) || 0,
          desvioPromedioDias: Number(data.desvioPromedioDias) || 0,
          tasaDescarte: Number(data.tasaDescarte) || 0,
          lotesExcluidos: Number(data.lotesExcluidos) || 0,
        };

        this.detalle = data.detalle || [];

        // Solo se grafican los lotes finalizados (los descartados no tienen desvío de cumplimiento).
        this.detalleGrafico = this.detalle.filter((l) => l.estado === 'Finalizado');
        // Eje X: solo el código (el estilo va en el tooltip) para no amontonar etiquetas.
        this.barChartLabels = this.detalleGrafico.map((l) => l.codigoLote);
        this.chartMinWidth = Math.max(this.detalleGrafico.length * 62, 600);
        this.barChartDatasets = [
          this.buildDataset(this.detalleGrafico.map((l) => l.diasEstimados), 'Estimado', this.COLOR_ESTIMADO),
          this.buildDataset(this.detalleGrafico.map((l) => l.diasReales), 'Real', this.COLOR_REAL),
        ];

        this.hayDatos = true;
        this.cargando = false;
      },
      error: (err: any) => {
        this.cargando = false;
        console.error(err);
      },
    });
  }

  descargarPdf(): void {
    const { fechaDesde, fechaHasta } = this.filtroForm.value;
    if (!fechaDesde || !fechaHasta) return;

    this.descargandoPdf = true;

    const graficoPrincipalBase64 = this.chart?.chart?.toBase64Image() || '';

    const payload = {
      fechaDesde,
      fechaHasta,
      tipoReporte: 'cumplimiento',
      graficoPrincipalBase64,
    };

    this.reporteService.descargarPdf(payload).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Reporte_Cumplimiento_${fechaDesde}_al_${fechaHasta}.pdf`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        this.descargandoPdf = false;
      },
      error: (err: any) => {
        console.error('Error al descargar el PDF', err);
        this.descargandoPdf = false;
      },
    });
  }
}
