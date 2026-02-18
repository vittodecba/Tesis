import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
// IMPORTAMOS LUCIDE Y EL ÍCONO DEL LÁPIZ
import { LucideAngularModule, Pencil } from 'lucide-angular'; 
import { RecetaService } from '../../../services/receta'; 

@Component({
  selector: 'app-receta-detalle',
  // AGREGAMOS LucideAngularModule A LOS IMPORTS
  imports: [CommonModule, RouterModule, LucideAngularModule], 
  templateUrl: './receta-detalle.html',
  styleUrl: './receta-detalle.css',
})
export class RecetaDetalle implements OnInit {
  receta: any = null;
  cargando: boolean = true;
  Pencil = Pencil; // AGREGAMOS EL ÍCONO A LA CLASE

  constructor(
    private route: ActivatedRoute,
    private recetaService: RecetaService
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = Number(idParam);
      this.cargarReceta(id);
    }
  }

  cargarReceta(id: number): void {
    this.recetaService.getRecetaDetalle(id).subscribe({
      next: (data: any) => {
        this.receta = data;
        this.cargando = false;
      },
      error: (err: any) => {
        console.error('Error al traer el detalle de la receta', err);
        this.cargando = false;
      }
    });
  }
}