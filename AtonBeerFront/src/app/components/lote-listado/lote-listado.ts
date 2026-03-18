import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router'; 
import { Lote } from '../../Interfaces/lote';
import { LoteService } from '../../services/lote';
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

  lotes: Lote[] = [];

  constructor(
    private router: Router,
    private loteService: LoteService
  ) { }

  ngOnInit(): void {
    this.cargarLotes();
  }

  cargarLotes() {
    const filtros = {
      fechaDesde: this.filtroFechaDesde,
      fechaHasta: this.filtroFechaHasta,
      estado: this.filtroEstado,
      recetaId: this.filtroReceta,
      fermentadorId: this.filtroFermentador
    };

    this.loteService.getLotes(filtros).subscribe({
      next: (data) => {
        this.lotes = data;
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  limpiarFiltros() {
    this.filtroLote = '';
    this.filtroEstado = '';
    this.filtroReceta = '';
    this.filtroFermentador = '';
    this.filtroFechaDesde = '';
    this.filtroFechaHasta = '';
    this.cargarLotes();
  }

  verDetalle(id: number) {
    this.router.navigate(['/planificacion/detalle', id]);
  }
}