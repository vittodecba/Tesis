import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Lote } from '../../Interfaces/lote';
import { LoteService } from '../../services/lote'; 
import { ArrowLeft, LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-lote-detalle',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './lote-detalle.html'
})
export class LoteDetalleComponent implements OnInit {
  lote: Lote | null = null;
  cargando: boolean = true;
  error: string | null = null;
  
  ArrowLeft = ArrowLeft;

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

  volver() {
    this.router.navigate(['/planificacion']);
  }
}