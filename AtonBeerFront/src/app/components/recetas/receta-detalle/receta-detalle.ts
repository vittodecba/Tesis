import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
// Agregamos Trash2 para borrar y Save para el botón de guardar
import { LucideAngularModule, Pencil, Plus, X, Beer, Trash2, Save } from 'lucide-angular'; 
import { Receta, RecetaService } from '../../../services/receta'; 
import { InsumoService } from '../../../services/insumo.service'; 

@Component({
  selector: 'app-receta-detalle',
  standalone: true,
  imports: [CommonModule, RouterModule, LucideAngularModule, FormsModule], 
  templateUrl: './receta-detalle.html',
  styleUrl: './receta-detalle.css',
})
export class RecetaDetalle implements OnInit {
  receta: Receta | null = null;
  cargando: boolean = true;  
  Pencil = Pencil; Plus = Plus; X = X; Beer = Beer; Trash2 = Trash2; Save = Save;
  listaInsumos: any[] = [];
  insumoIdSeleccionado: number = 0;
  cantidadIngresada: number = 0;
  mostrarFormInsumo: boolean = false; 

  constructor(
    private route: ActivatedRoute,
    private recetaService: RecetaService,
    private insumoService: InsumoService 
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = Number(idParam);
      this.cargarReceta(id);
      this.cargarInsumosDisponibles();
    }
  }

  cargarReceta(id: number): void {
    this.recetaService.getRecetaDetalle(id).subscribe({
      next: (data: any) => {
        this.receta = data;
        this.cargando = false;
      },
      error: (err: any) => {
        console.error('Error al traer el detalle', err);
        this.cargando = false;
      }
    });
  }

  cargarInsumosDisponibles(): void {
    this.insumoService.obtenerInsumos().subscribe({
      next: (data) => this.listaInsumos = data,
      error: (err) => console.error('Error al cargar insumos', err)
    });
  }

  agregarInsumoDinamico(): void {
    if (!this.receta || this.insumoIdSeleccionado === 0 || this.cantidadIngresada <= 0) {
      alert('Datos inválidos');
      return;
    }

    const dto = {
      insumoId: Number(this.insumoIdSeleccionado),
      cantidad: this.cantidadIngresada
    };

    this.recetaService.addInsumo(this.receta.idReceta, dto).subscribe({
      next: () => {
        this.cargarReceta(this.receta!.idReceta); // Recarga la tabla para ver el nuevo
        this.resetFormInsumo();
      },
      error: (err) => alert('Error al agregar el insumo')
    });
  }

  eliminarInsumo(insumoId: number): void {
    if (confirm('¿Estás seguro de quitar este ingrediente?')) {
      this.recetaService.removeInsumo(this.receta!.idReceta, insumoId).subscribe({
        next: () => {
          this.cargarReceta(this.receta!.idReceta); // Recarga la tabla tras borrar
        },
        error: (err) => alert('No se pudo eliminar el insumo')
      });
    }
  }

  private resetFormInsumo() {
    this.insumoIdSeleccionado = 0;
    this.cantidadIngresada = 0;
    this.mostrarFormInsumo = false;
  }
}