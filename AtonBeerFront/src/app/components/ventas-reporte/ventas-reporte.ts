import { Component, inject, OnInit, ViewChildren, QueryList } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { BaseChartDirective } from 'ng2-charts';
import { ChartOptions, Chart, registerables } from 'chart.js';
import { VentasService, ReporteVentas } from '../../services/ventas.service';

Chart.register(...registerables);

@Component({
  selector: 'app-ventas-reporte',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, BaseChartDirective],
  templateUrl: './ventas-reporte.html',
  styleUrl: './ventas-reporte.css',
})
export class VentasReporte implements OnInit {
  @ViewChildren(BaseChartDirective) charts?: QueryList<BaseChartDirective>;

  filtroForm: FormGroup;
  tabActiva: string = 'general';
  cargando: boolean = false;
  descargandoPdf: boolean = false;
  mostrarComparativaDiaria: boolean = true;
  
  resumen = {
    totalVendido: 0,
    cantidadVentas: 0,
    efectivoTotal: 0,
    transferenciaTotal: 0,
    ticketPromedio: 0,
    variacionIngresosPorcentaje: 0
  };

  topClientes: any[] = [];
  topProductos: any[] = [];
  topEstilos: any[] = [];

  public barChartOptions: ChartOptions<'bar'> = { responsive: true, maintainAspectRatio: false };
  public barChartLabels: string[] = [];
  public barChartDatasets: any[] = [
    { data: [], label: 'Período Actual', backgroundColor: '#e67e22' },
    { data: [], label: 'Período Anterior', backgroundColor: '#d1d5db' }
  ];

  public mensualBarChartOptions: ChartOptions<'bar'> = { 
    responsive: true, 
    maintainAspectRatio: false,
    scales: {
      x: { title: { display: true, text: 'Meses' } },
      y: { title: { display: true, text: 'Ingreso Total ($)' }, beginAtZero: true }
    }
  };
  public mensualBarChartLabels: string[] = [];
  public mensualBarChartDatasets: any[] = [
    { data: [], label: 'Ingresos Mensuales', backgroundColor: '#3b82f6' }
  ];

  public doughnutChartOptions: ChartOptions<'doughnut'> = { 
    responsive: true, 
    maintainAspectRatio: false, 
    plugins: { legend: { position: 'bottom' } } 
  };
  public doughnutChartLabels: string[] = ['Efectivo', 'Transferencia'];
  public doughnutChartDatasets: any[] = [
    { data: [], backgroundColor: ['#22c55e', '#3b82f6'], hoverBackgroundColor: ['#16a34a', '#2563eb'] }
  ];

  public scatterChartOptions: ChartOptions<'scatter'> = { 
    responsive: true, 
    maintainAspectRatio: false,
    scales: { 
      x: { title: { display: true, text: 'Cantidad de Pedidos' } }, 
      y: { title: { display: true, text: 'Monto Total Comprado ($)' } } 
    },
    plugins: {
      tooltip: {
        callbacks: {
          label: function(context: any) {
            const data = context.raw;
            return `${data.cliente} - Pedidos: ${data.x}, Total: $${data.y}`;
          }
        }
      }
    }
  };
  public scatterChartDatasets: any[] = [
    { data: [], label: 'Clientes', backgroundColor: '#8b5cf6', pointRadius: 6, pointHoverRadius: 8 }
  ];

  public lineChartOptions: ChartOptions<'line'> = { 
    responsive: true, 
    maintainAspectRatio: false,
    scales: {
      x: { title: { display: true, text: 'Fechas' } },
      y: { title: { display: true, text: 'Unidades Vendidas' }, beginAtZero: true }
    }
  };
  public lineChartLabels: string[] = [];
  public lineChartDatasets: any[] = [
    { data: [], label: 'Estilos', borderColor: '#e67e22', tension: 0.3, fill: false }
  ];

  private fb = inject(FormBuilder);
  private ventasService = inject(VentasService);

  constructor() {
    this.filtroForm = this.fb.group({
      periodoRapido: [''],
      fechaDesde: [''],
      fechaHasta: [''],
      clienteId: ['']
    });
  }

  ngOnInit(): void {
    this.filtroForm.get('periodoRapido')?.valueChanges.subscribe(valor => {
      const hoy = new Date();
      let desde = '';
      let hasta = '';

      if (valor === 'mes_actual') {
        desde = new Date(hoy.getFullYear(), hoy.getMonth(), 1).toISOString().split('T')[0];
        hasta = new Date(hoy.getFullYear(), hoy.getMonth() + 1, 0).toISOString().split('T')[0];
      } else if (valor === 'mes_anterior') {
        desde = new Date(hoy.getFullYear(), hoy.getMonth() - 1, 1).toISOString().split('T')[0];
        hasta = new Date(hoy.getFullYear(), hoy.getMonth(), 0).toISOString().split('T')[0];
      }

      if (desde && hasta) {
        this.filtroForm.patchValue({ fechaDesde: desde, fechaHasta: hasta });
      }
    });
  }

  cambiarTab(tab: string): void {
    this.tabActiva = tab;
    setTimeout(() => {
      this.charts?.forEach(chart => {
        if (chart && chart.chart) {
          chart.chart.update();
        }
      });
    }, 50);
  }

  filtrarReporte(): void {
    const { fechaDesde, fechaHasta } = this.filtroForm.value;
    if (!fechaDesde || !fechaHasta) return;

    this.cargando = true;

    const dDesde = new Date(fechaDesde + 'T00:00:00');
    const dHasta = new Date(fechaHasta + 'T00:00:00');

    const diffTime = Math.abs(dHasta.getTime() - dDesde.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    this.mostrarComparativaDiaria = diffDays <= 31;

    const dDesdeAnt = new Date(dDesde);
    dDesdeAnt.setMonth(dDesdeAnt.getMonth() - 1);
    const dHastaAnt = new Date(dHasta);
    dHastaAnt.setMonth(dHastaAnt.getMonth() - 1);

    const formatStr = (d: Date) => d.toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit' });
    const labelActual = `Actual (${formatStr(dDesde)} al ${formatStr(dHasta)})`;
    const labelAnterior = `Anterior (${formatStr(dDesdeAnt)} al ${formatStr(dHastaAnt)})`;

    this.ventasService.obtenerReporteVentas(fechaDesde, fechaHasta).subscribe({
      next: (data: ReporteVentas) => {
        this.resumen = {
          totalVendido: data.totalVendido,
          cantidadVentas: data.cantidadVentas,
          efectivoTotal: data.efectivoTotal,
          transferenciaTotal: data.transferenciaTotal,
          ticketPromedio: data.ticketPromedio,
          variacionIngresosPorcentaje: data.variacionIngresosPorcentaje
        };

        this.topClientes = data.topClientes.slice(0, 5);
        this.topProductos = data.topProductos;
        this.topEstilos = data.topEstilos;

        if (this.mostrarComparativaDiaria) {
          const diasDelMes = Array.from({length: 31}, (_, i) => i + 1);

          this.barChartLabels = diasDelMes.map(d => `Día ${d}`);
          this.barChartDatasets = [
            { 
              data: diasDelMes.map(d => {
                const dia = data.comparativaMensual.find(c => c.diaDelPeriodo === d);
                return dia ? dia.totalActual : 0;
              }), 
              label: labelActual, 
              backgroundColor: '#e67e22' 
            },
            { 
              data: diasDelMes.map(d => {
                const dia = data.comparativaMensual.find(c => c.diaDelPeriodo === d);
                return dia ? dia.totalAnterior : 0;
              }), 
              label: labelAnterior, 
              backgroundColor: '#d1d5db' 
            }
          ];
        }

        this.mensualBarChartLabels = data.evolucionMensualIngresos.map(m => m.mes);
        this.mensualBarChartDatasets = [
          { data: data.evolucionMensualIngresos.map(m => m.total), label: 'Ingresos Mensuales', backgroundColor: '#3b82f6' }
        ];

        this.doughnutChartDatasets = [
          { data: [data.efectivoTotal, data.transferenciaTotal], backgroundColor: ['#22c55e', '#3b82f6'], hoverBackgroundColor: ['#16a34a', '#2563eb'] }
        ];

        this.scatterChartDatasets = [
          { 
            data: data.topClientes.map(c => ({ 
              x: c.cantidadVentas, 
              y: c.totalComprado,
              cliente: c.cliente
            })), 
            label: 'Clientes', 
            backgroundColor: '#8b5cf6', 
            pointRadius: 6, 
            pointHoverRadius: 8 
          }
        ];

        const estilos = [...new Set(data.evolucionEstilos.map(e => e.estilo))];
        const fechas = [...new Set(data.evolucionEstilos.map(e => e.fecha))].sort();
        const colores = ['#e67e22', '#3b82f6', '#22c55e', '#ef4444', '#a855f7'];

        this.lineChartLabels = fechas;
        this.lineChartDatasets = estilos.map((estilo, i) => ({
          label: estilo,
          data: fechas.map(f => {
            const registro = data.evolucionEstilos.find(e => e.fecha === f && e.estilo === estilo);
            return registro ? registro.cantidad : 0;
          }),
          borderColor: colores[i % colores.length],
          tension: 0.3,
          fill: false
        }));

        this.cargando = false;

        setTimeout(() => {
          this.charts?.forEach(chart => {
            if (chart && chart.chart) {
              chart.chart.update();
            }
          });
        }, 50);
      },
      error: (err: any) => {
        this.cargando = false;
        console.error(err);
      }
    });
  }

  descargarPdf(): void {
    const { fechaDesde, fechaHasta } = this.filtroForm.value;
    if (!fechaDesde || !fechaHasta) return;

    this.descargandoPdf = true;
    this.ventasService.descargarPdfReporte(fechaDesde, fechaHasta).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Reporte_Ventas_${fechaDesde}_al_${fechaHasta}.pdf`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        this.descargandoPdf = false;
      },
      error: (err: any) => {
        console.error('Error al descargar el PDF', err);
        this.descargandoPdf = false;
      }
    });
  }
}