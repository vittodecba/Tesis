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
  };

  detalle: LoteCumplimiento[] = [];

  public barChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: { legend: { position: 'top' } },
    scales: {
      y: { beginAtZero: true, title: { display: true, text: 'Días' } },
    },
  };
  public barChartLabels: string[] = [];
  public barChartDatasets: any[] = [
    { data: [], label: 'Estimado', backgroundColor: '#9ca3af' },
    { data: [], label: 'Real', backgroundColor: '#E67E22' },
  ];

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
        };

        this.detalle = data.detalle || [];

        this.barChartLabels = this.detalle.map((l) =>
          `${l.codigoLote}${l.estilo ? ' ' + l.estilo : ''}`
        );
        this.barChartDatasets = [
          { data: this.detalle.map((l) => l.diasEstimados), label: 'Estimado', backgroundColor: '#9ca3af' },
          { data: this.detalle.map((l) => l.diasReales), label: 'Real', backgroundColor: '#E67E22' },
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
