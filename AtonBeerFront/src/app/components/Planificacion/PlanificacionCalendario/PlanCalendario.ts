import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FullCalendarModule } from '@fullcalendar/angular';
import { CalendarOptions } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin from '@fullcalendar/interaction';
import esLocale from '@fullcalendar/core/locales/es';
import { PlanificacionService } from '../../../services/PlanificacionService';
import timeGridPlugin from '@fullcalendar/timegrid';
declare var bootstrap: any;//prueba
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
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
    locale: esLocale,
    eventDisplay: 'block',    
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,timeGridWeek'
    },
    events: [],    
   eventContent: (arg: any) => this.renderEvento(arg),
    eventClick: (info: any) => {
  console.log("Lote seleccionado:", info.event.extendedProps.loteId);
} 
  };

  constructor(private _planifService: PlanificacionService) {}

  ngOnInit(): void {
    this.cargarDatos();
  }

  cargarDatos() {
    this._planifService.getPlanificaciones().subscribe({
      next: (data) => {
        console.log('fechaFin raw:', data[0]?.fechaFinEstimada);
        console.log('fechaFin substring:', data[0]?.fechaFinEstimada?.substring(0, 10));        
        const colores = this.estadoConfig;
        this.calendarOptions = {
          ...this.calendarOptions,
          events: data.map(p => {
            const fechaFinStr = p.fechaFinEstimada.substring(0, 10); // "2026-04-18"
            const [year, month, day] = fechaFinStr.split('-').map(Number);
            const fechaFin = new Date(year, month - 1, day + 1); // mes es 0-indexed
            const endStr = `${fechaFin.getFullYear()}-${String(fechaFin.getMonth()+1).padStart(2,'0')}-${String(fechaFin.getDate()).padStart(2,'0')}`;
            console.log('endStr calculado:', endStr);
            return{
            id: String(p.loteId),
            title: `Lote #${p.loteId}`,
            start: p.fechaInicio,
            end: endStr,
            backgroundColor: colores[p.estado]?.bg ?? '#6c757d',
            borderColor: colores[p.estado]?.color ?? '#6c757d',
            textColor: colores[p.estado]?.color ?? '#ffffff',
            extendedProps: {
              loteId: p.loteId,
              volumen: p.volumenLitros,
              recetaId: p.recetaId,
              estado: p.estado,
              estadoNombre: colores[p.estado]?.nombre ?? 'Desconocido',
              estadoIcono: colores[p.estado]?.icono ?? '?',
            }
         }})
        };
      }
    });
  }

  // Tarjeta visual personalizada
  renderEvento(arg: any) {
    const p = arg.event.extendedProps;
    const color = this.estadoConfig[p.estado]?.color ?? '#6c757d';
    const bg = this.estadoConfig[p.estado]?.bg ?? '#f8f9fa';

    // 🔴 SI NO ES EL DÍA DE INICIO: Retornamos una barra de color sólida y vacía
    if (!arg.isStart) {
      return { 
        html: `<div style="background: ${bg}; height: 40px; width: 100%; border-top: 1px solid ${color}22; border-bottom: 1px solid ${color}22;"></div>` 
      };
    }

    // 🟢 SI ES EL DÍA DE INICIO: Retornamos la tarjetita con todos los datos
    return {
      html: `
        <div class="lote-card shadow-sm" style="border-left: 4px solid ${color}; background: ${bg}; min-height: 40px;">
          <div class="lote-id" style="color: ${color}">#L${String(p.loteId).padStart(3, '0')}</div>
          <div class="lote-vol">🧪 ${p.volumen}L</div>
          <div class="lote-estado" style="color: ${color}; font-size: 9px;">${p.estadoIcono} ${p.estadoNombre}</div>
        </div>
      `
    };
  }
}
