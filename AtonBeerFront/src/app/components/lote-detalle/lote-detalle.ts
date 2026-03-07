import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Lote } from '../../Interfaces/lote';
import { LoteService } from '../../services/lote'; 
import { ArrowLeft, Edit2, Check, X, LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-lote-detalle',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './lote-detalle.html'
})
export class LoteDetalleComponent implements OnInit {
  lote: Lote | null = null;
  cargando: boolean = true;
  error: string | null = null;
  
  // --- Variables para el fermentador ---
  fermentadoresDisponibles: any[] = [];
  editandoFermentador: boolean = false;
  nuevoFermentadorId: number | string = '';
  guardandoFermentador: boolean = false;
  
  ArrowLeft = ArrowLeft;
  Edit2 = Edit2;
  Check = Check;
  X = X;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private loteService: LoteService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.cargarLote(Number(id));
    } else {
      this.error = 'No se proporcionó un ID de lote válido.';
      this.cargando = false;
    }
  }

  cargarLote(id: number) {
    this.loteService.getLoteById(id).subscribe({
      next: (data) => {
        this.lote = data;
        this.cargando = false;
      },
      error: (err) => {
        this.error = 'Error al cargar el lote. Puede que no exista o el servidor esté apagado.';
        this.cargando = false;
      }
    });
  }

  // --- MÉTODOS PARA EL FERMENTADOR ---
  cargarFermentadores() {
    this.loteService.getFermentadoresDisponibles().subscribe({
      next: (data) => this.fermentadoresDisponibles = data,
      error: (err) => console.error('Error al cargar fermentadores libres', err)
    });
  }

  iniciarEdicionFermentador() {
    this.editandoFermentador = true;
    this.nuevoFermentadorId = ''; 
    this.cargarFermentadores();
  }

  cancelarEdicionFermentador() {
    this.editandoFermentador = false;
  }

  guardarFermentador() {
    if (!this.nuevoFermentadorId || !this.lote) return;
    
    this.guardandoFermentador = true;
    this.loteService.asignarFermentador(this.lote.id, Number(this.nuevoFermentadorId)).subscribe({
      next: () => {
        this.cargarLote(this.lote!.id); // Recargamos para ver el cambio impactado
        this.editandoFermentador = false;
        this.guardandoFermentador = false;
      },
      error: (err) => {
        alert('Error: El backend todavía no está listo para guardar el fermentador.');
        this.editandoFermentador = false;
        this.guardandoFermentador = false;
      }
    });
  }
  // ------------------------------------------

  volver() {
    this.router.navigate(['/planificacion']);
  }
}