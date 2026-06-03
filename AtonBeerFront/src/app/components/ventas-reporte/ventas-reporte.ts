import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartOptions } from 'chart.js';
import { VentasService, ReporteVentas, TopCliente, TopProducto, TopEstilo } from '../../services/ventas.service';

@Component({
  selector: 'app-ventas-reporte',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, BaseChartDirective],
  templateUrl: './ventas-reporte.html',
  styleUrl: './ventas-reporte.css',
})
export class VentasReporte implements OnInit {
  filtroForm: FormGroup;
  tabActiva: string = 'general';
  
  resumen = {
    totalVendido: 0,
    cantidadVentas: 0,
    efectivoTotal: 0,
    transferenciaTotal: 0,
    ticketPromedio: 0,
    variacionIngresosPorcentaje: 0
  };

  topClientes: TopCliente[] = [];
  topProductos: TopProducto[] = [];
  topEstilos: TopEstilo[] = [];

  public barChartLegend = true;
  public barChartPlugins = [];
  
  public barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [
      { data: [], label: 'Ingresos por Día ($)', backgroundColor: '#e67e22' }
    ]
  };

  public barChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false
  };

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
        const primerDia = new Date(hoy.getFullYear(), hoy.getMonth(), 1);
        const ultimoDia = new Date(hoy.getFullYear(), hoy.getMonth() + 1, 0);
        desde = primerDia.toISOString().split('T')[0];
        hasta = ultimoDia.toISOString().split('T')[0];
      } else if (valor === 'mes_anterior') {
        const primerDia = new Date(hoy.getFullYear(), hoy.getMonth() - 1, 1);
        const ultimoDia = new Date(hoy.getFullYear(), hoy.getMonth(), 0);
        desde = primerDia.toISOString().split('T')[0];
        hasta = ultimoDia.toISOString().split('T')[0];
      }

      if (desde && hasta) {
        this.filtroForm.patchValue({
          fechaDesde: desde,
          fechaHasta: hasta
        });
      }
    });
  }

  cambiarTab(tab: string): void {
    this.tabActiva = tab;
  }

  filtrarReporte(): void {
    const { fechaDesde, fechaHasta } = this.filtroForm.value;
    
    if (!fechaDesde || !fechaHasta) {
      return;
    }

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

        this.topClientes = data.topClientes;
        this.topProductos = data.topProductos;
        this.topEstilos = data.topEstilos;

        this.barChartData = {
          labels: data.ventasPorDia.map((v) => v.fecha),
          datasets: [
            { 
              data: data.ventasPorDia.map((v) => v.total), 
              label: 'Ingresos por Día ($)', 
              backgroundColor: '#e67e22' 
            }
          ]
        };
      },
      error: (err: any) => {
        console.error(err);
      }
    });
  }
}