import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FullCalendarModule } from '@fullcalendar/angular';
import { CalendarOptions } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin from '@fullcalendar/interaction'; // 👈 Necesario para drag & drop
import esLocale from '@fullcalendar/core/locales/es';
import { PlanificacionService } from '../../../services/PlanificacionService';
import timeGridPlugin from '@fullcalendar/timegrid';

@Component({
  selector: 'app-planificacion-calendar',
  standalone: true,
  imports: [CommonModule, FullCalendarModule],
  templateUrl: './PlanCalendario.html',
  styleUrls: ['./PlanCalendario.scss']
})
export class PlanificacionCalendarComponent implements OnInit {

  // Colores por estado
  private estadoConfig: { [key: number]: { color: string, bg: string, nombre: string, icono: string } } = {
    0: { color: '#dc3545', bg: '#fff5f5', nombre: 'Cancelado',   icono: '✕' },
    1: { color: '#0d6efd', bg: '#f0f5ff', nombre: 'Planificado', icono: '📋' },
    2: { color: '#ffc107', bg: '#fffbf0', nombre: 'En Proceso',  icono: '⚗️' },
    3: { color: '#198754', bg: '#f0fff4', nombre: 'Finalizado',  icono: '✓' },
  };

  calendarOptions: CalendarOptions = {
    initialView: 'dayGridMonth',
    plugins: [dayGridPlugin,timeGridPlugin ,interactionPlugin],
    locale: esLocale,
    eventDisplay: 'block',    
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth, timeGridWeek'
    },
    events: [],    
    eventContent: (arg: any) => this.renderEvento(arg), 
  };

  constructor(private _planifService: PlanificacionService) {}

  ngOnInit(): void {
    this.cargarDatos();
  }

  cargarDatos() {
    this._planifService.getPlanificaciones().subscribe({
      next: (data) => {
        const colores = this.estadoConfig;
        this.calendarOptions = {
          ...this.calendarOptions,
          events: data.map(p => ({
            id: String(p.loteId),
            title: `Lote #${p.loteId}`,
            start: p.fechaInicio,
            end: p.fechaFinEstimada,
            backgroundColor: colores[p.estado]?.bg ?? '#f8f9fa',
            borderColor: colores[p.estado]?.color ?? '#6c757d',
            textColor: colores[p.estado]?.color ?? '#6c757d',
            extendedProps: {
              loteId: p.loteId,
              volumen: p.volumenLitros,
              recetaId: p.recetaId,
              estado: p.estado,
              estadoNombre: colores[p.estado]?.nombre ?? 'Desconocido',
              estadoIcono: colores[p.estado]?.icono ?? '?',
            }
          }))
        };
      }
    });
  }

  // Tarjeta visual personalizada
  renderEvento(arg: any) {
    const p = arg.event.extendedProps;
    const color = this.estadoConfig[p.estado]?.color ?? '#6c757d';
    return {
      html: `
        <div class="lote-card" style="border-left: 3px solid ${color}; background: ${this.estadoConfig[p.estado]?.bg}">
          <div class="lote-id">#L${String(p.loteId).padStart(3, '0')}</div>
          <div class="lote-vol">🧪 ${p.volumen}L</div>
          <div class="lote-estado" style="color: ${color}">${p.estadoIcono} ${p.estadoNombre}</div>
        </div>
      `
    };
  }
}
