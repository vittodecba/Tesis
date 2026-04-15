import Swal from 'sweetalert2';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartOptions, Chart, registerables } from 'chart.js';

import { Fermentador } from '../../Interfaces/fermentador';
import { Lote } from '../../Interfaces/lote';
import { RegistroFermentacion } from '../../Interfaces/registro-fermentacion';
import { FermentadorService } from '../../services/fermentador';
import { LoteService } from '../../services/lote';
import { RegistroFermentacionService } from '../../services/registro-fermentacion';
import {
  LucideAngularModule,
  ArrowLeft,
  Save,
  Beaker,
  ClipboardList,
  LineChart,
} from 'lucide-angular';

Chart.register(...registerables);

@Component({
  selector: 'app-fermentador-detalle',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, BaseChartDirective],
  templateUrl: './fermentador-detalle.html',
})
export class FermentadorDetalleComponent implements OnInit {
  readonly ArrowLeft = ArrowLeft;
  readonly Save = Save;
  readonly Beaker = Beaker;
  readonly ClipboardList = ClipboardList;
  readonly LineChart = LineChart;

  cargando = true;
  guardandoRegistro = false;

  fermentadorId!: number;
  fermentador: Fermentador | null = null;
  loteActivo: Lote | null = null;
  registros: RegistroFermentacion[] = [];

  errorCarga = '';
  errorRegistro = '';
  mensajeExito = '';

  nuevoRegistro: RegistroFermentacion = this.crearRegistroVacio();

  temperaturaChartData: ChartConfiguration<'line'>['data'] = {
    labels: [],
    datasets: [
      {
        label: 'Temperatura (°C)',
        data: [],
        tension: 0.25,
        fill: false,
      },
    ],
  };

  phChartData: ChartConfiguration<'line'>['data'] = {
    labels: [],
    datasets: [
      {
        label: 'pH',
        data: [],
        tension: 0.25,
        fill: false,
      },
    ],
  };

  lineChartOptions: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
      },
      tooltip: {
        enabled: true,
      },
    },
    scales: {
      x: {
        title: {
          display: true,
          text: 'Día de fermentación',
        },
      },
      y: {
        beginAtZero: false,
      },
    },
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fermentadorService: FermentadorService,
    private loteService: LoteService,
    private registroService: RegistroFermentacionService,
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.fermentadorId = Number(idParam);

    if (!this.fermentadorId || isNaN(this.fermentadorId)) {
      this.errorCarga = 'ID de fermentador inválido.';
      this.cargando = false;
      return;
    }

    this.cargarPantalla();
  }

  crearRegistroVacio(): RegistroFermentacion {
    return {
      loteId: 0,
      fecha: '',
      diaFermentacion: 1,
      ph: 0,
      densidad: 0,
      temperatura: 0,
      presion: 0,
      purgas: '',
      extracciones: '',
      agregados: '',
      observaciones: '',
    };
  }

  getFechaLocalISO(): string {
    const ahora = new Date();
    const year = ahora.getFullYear();
    const month = String(ahora.getMonth() + 1).padStart(2, '0');
    const day = String(ahora.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}T00:00:00`;
  }

  volver() {
    this.router.navigate(['/fermentadores']);
  }

  cargarPantalla() {
    this.cargando = true;
    this.errorCarga = '';
    this.mensajeExito = '';

    this.fermentadorService.getFermentadores().subscribe({
      next: (fermentadores) => {
        this.fermentador = fermentadores.find((f) => f.id === this.fermentadorId) ?? null;

        if (!this.fermentador) {
          this.errorCarga = 'No se encontró el fermentador.';
          this.cargando = false;
          return;
        }

        this.cargarLoteActivo();
      },
      error: (err) => {
        console.error(err);
        this.errorCarga = 'No se pudo cargar el fermentador.';
        this.cargando = false;
      },
    });
  }

  cargarLoteActivo() {
    this.loteService.getLoteActivoByFermentadorId(this.fermentadorId).subscribe({
      next: (lote) => {
        this.loteActivo = lote;
        this.cargarRegistros();
      },
      error: () => {
        this.loteActivo = null;
        this.registros = [];
        this.actualizarGraficos();
        this.cargando = false;
      },
    });
  }

  cargarRegistros() {
    if (!this.loteActivo) {
      this.registros = [];
      this.actualizarGraficos();
      this.cargando = false;
      return;
    }

    this.registroService.getByLoteId(this.loteActivo.id).subscribe({
      next: (data) => {
        this.registros = [...data].sort((a, b) => a.diaFermentacion - b.diaFermentacion);
        this.prepararNuevoRegistro();
        this.actualizarGraficos();
        this.cargando = false;
      },
      error: (err) => {
        console.error(err);
        this.errorCarga = 'No se pudieron cargar los registros de fermentación.';
        this.actualizarGraficos();
        this.cargando = false;
      },
    });
  }

  prepararNuevoRegistro() {
    if (!this.loteActivo) return;

    const siguienteDia =
      this.registros.length > 0 ? Math.max(...this.registros.map((r) => r.diaFermentacion)) + 1 : 1;

    this.nuevoRegistro = {
      loteId: this.loteActivo.id,
      fecha: this.getFechaLocalISO(),
      diaFermentacion: siguienteDia,
      ph: 0,
      densidad: 0,
      temperatura: 0,
      presion: 0,
      purgas: '',
      extracciones: '',
      agregados: '',
      observaciones: '',
    };
  }

  guardarRegistro() {
    if (!this.loteActivo) return;

    this.errorRegistro = '';
    this.mensajeExito = '';

    if (!this.nuevoRegistro.fecha) {
      this.errorRegistro = 'La fecha es obligatoria.';
      return;
    }

    if (!this.nuevoRegistro.diaFermentacion || this.nuevoRegistro.diaFermentacion <= 0) {
      this.errorRegistro = 'El día de fermentación es obligatorio y debe ser mayor a 0.';
      return;
    }

    if (this.nuevoRegistro.temperatura <= 0) {
      this.errorRegistro = 'La temperatura es obligatoria y debe ser mayor a 0.';
      return;
    }

    if (this.nuevoRegistro.densidad < 0) {
      this.errorRegistro = 'La densidad no puede ser negativa.';
      return;
    }

    if (this.nuevoRegistro.ph <= 0) {
      this.errorRegistro = 'El pH es obligatorio y debe ser mayor a 0.';
      return;
    }

    if (this.nuevoRegistro.presion === null || this.nuevoRegistro.presion === undefined) {
      this.errorRegistro = 'La presión es obligatoria.';
      return;
    }

    if (this.nuevoRegistro.presion < 0) {
      this.errorRegistro = 'La presión no puede ser negativa.';
      return;
    }

    this.guardandoRegistro = true;

    const payload: RegistroFermentacion = {
      ...this.nuevoRegistro,
      loteId: this.loteActivo.id,
      purgas: this.nuevoRegistro.purgas?.trim() || '',
      extracciones: this.nuevoRegistro.extracciones?.trim() || '',
      agregados: this.nuevoRegistro.agregados?.trim() || '',
      observaciones: this.nuevoRegistro.observaciones?.trim() || '',
    };

    this.registroService.crearRegistro(payload).subscribe({
      next: () => {
        this.guardandoRegistro = false;
        this.mensajeExito = 'Registro guardado correctamente.';
        this.cargarRegistros();
      },
      error: (err) => {
        console.error(err);
        this.guardandoRegistro = false;
        this.errorRegistro = err?.error || 'No se pudo guardar el registro.';
      },
    });
  }

  async finalizarLote() {
    if (!this.loteActivo) return;

    const result = await Swal.fire({
      title: '¿Cómo querés cerrar este lote?',
      text: 'Elegí el motivo de cierre.',
      icon: 'question',
      showDenyButton: true,
      showCancelButton: true,
      confirmButtonText: '✅ Completado',
      denyButtonText: '⚠️ Descartado / Problema',
      cancelButtonText: 'Cancelar',
      confirmButtonColor: '#198754',
      denyButtonColor: '#dc3545',
    });

    if (result.isDismissed) return;

    // Finalizado = 3, Descartado = 4
    const estadoFinal = result.isConfirmed ? 3 : 4;
    const mensajeOk = result.isConfirmed
      ? 'Lote finalizado correctamente.'
      : 'Lote descartado correctamente.';

    this.loteService.finalizarLote(this.loteActivo.id, estadoFinal).subscribe({
      next: () => {
        this.mensajeExito = mensajeOk;
        this.cargarPantalla();
      },
      error: (err) => {
        console.error(err);
        this.errorCarga = err?.error || 'No se pudo finalizar el lote.';
      },
    });
  }

  actualizarGraficos() {
    const labels = this.registros.map((r) => `Día ${r.diaFermentacion}`);
    const temperaturas = this.registros.map((r) => r.temperatura);
    const phs = this.registros.map((r) => r.ph);

    this.temperaturaChartData = {
      labels,
      datasets: [
        {
          label: 'Temperatura (°C)',
          data: temperaturas,
          tension: 0.25,
          fill: false,
        },
      ],
    };

    this.phChartData = {
      labels,
      datasets: [
        {
          label: 'pH',
          data: phs,
          tension: 0.25,
          fill: false,
        },
      ],
    };
  }

  getEstadoTexto(estado: string | undefined): string {
    const mapa: Record<string, string> = {
      '1': 'Disponible',
      '2': 'Ocupado',
      '3': 'Sucio',
      '4': 'Mantenimiento',
      Disponible: 'Disponible',
      Ocupado: 'Ocupado',
      Sucio: 'Sucio',
      Mantenimiento: 'Mantenimiento',
    };

    return mapa[estado ?? ''] ?? 'Desconocido';
  }

  getEstadoClase(estado: string | undefined): string {
    const mapa: Record<string, string> = {
      '1': 'bg-green-100 text-green-700',
      '2': 'bg-blue-100 text-blue-700',
      '3': 'bg-yellow-100 text-yellow-700',
      '4': 'bg-red-100 text-red-700',
      Disponible: 'bg-green-100 text-green-700',
      Ocupado: 'bg-blue-100 text-blue-700',
      Sucio: 'bg-yellow-100 text-yellow-700',
      Mantenimiento: 'bg-red-100 text-red-700',
    };

    return mapa[estado ?? ''] ?? 'bg-gray-100 text-gray-700';
  }
}
