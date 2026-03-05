import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Lote } from '../../Interfaces/lote';
import { Plus, Search, FileText, LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-lote-listado',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './lote-listado.html',
  styleUrls: ['./lote-listado.css']
})
export class LoteListadoComponent implements OnInit {
  filtroLote: string = '';
  filtroEstado: string = '';
  filtroReceta: string = '';
  filtroFermentador: string = '';
  filtroFechaDesde: string = '';
  filtroFechaHasta: string = '';

  listaRecetas: string[] = ['Red Ale "Fuego Rojo"', 'Pilsen Tradicional'];
  listaFermentadores: string[] = ['F-01', 'F-02', 'F-03'];

  Plus = Plus;
  Search = Search;
  FileText = FileText;

  lotesRaw: Lote[] = [
    { 
      id: 1, 
      numeroLote: 'L-001', 
      estilo: 'Red Ale "Fuego Rojo"', 
      volumenPlanificado: 15, 
      fechaInicioPlanificada: new Date('2025-10-10'), 
      fechaFinEstimada: new Date('2025-10-25'), 
      estado: 'En curso',
      fermentadorNombre: 'F-01'
    },
    { 
      id: 2, 
      numeroLote: 'L-002', 
      estilo: 'Pilsen Tradicional', 
      volumenPlanificado: 20, 
      fechaInicioPlanificada: new Date('2025-10-15'), 
      fechaFinEstimada: new Date('2025-11-05'), 
      estado: 'Pendiente'
    }
  ];

  constructor() { }

  ngOnInit(): void { }

  get lotes() {
    return this.lotesRaw.filter(lote => {
      const matchLote = this.filtroLote === '' || lote.numeroLote.toLowerCase().includes(this.filtroLote.toLowerCase());
      const matchEstado = this.filtroEstado === '' || lote.estado === this.filtroEstado;
      const matchReceta = this.filtroReceta === '' || lote.estilo === this.filtroReceta;
      const matchFermentador = this.filtroFermentador === '' || lote.fermentadorNombre === this.filtroFermentador;
      
      let matchFecha = true;
      if (this.filtroFechaDesde && this.filtroFechaHasta) {
        const desde = new Date(this.filtroFechaDesde).getTime();
        const hasta = new Date(this.filtroFechaHasta).getTime();
        const fechaLote = new Date(lote.fechaInicioPlanificada).getTime();
        matchFecha = fechaLote >= desde && fechaLote <= hasta;
      }

      return matchLote && matchEstado && matchReceta && matchFermentador && matchFecha;
    });
  }

  limpiarFiltros() {
    this.filtroLote = '';
    this.filtroEstado = '';
    this.filtroReceta = '';
    this.filtroFermentador = '';
    this.filtroFechaDesde = '';
    this.filtroFechaHasta = '';
  }
}