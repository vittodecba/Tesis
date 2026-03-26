import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Lote } from '../../Interfaces/lote';
import { LoteService } from '../../services/lote';
import { FermentadorService } from '../../services/fermentador';
import { ArrowLeft, Edit2, Check, X, LucideAngularModule } from 'lucide-angular';
import { AuthService } from '../../core/services/auth.service';
import { RecetaService, Receta } from '../../services/receta';

@Component({
  selector: 'app-lote-detalle',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './lote-detalle.html'
})
export class LoteDetalleComponent implements OnInit {
  lote: any | null = null;
  insumos: any[] = [];
  cargando: boolean = true;
  cargandoInsumos: boolean = false;
  error: string | null = null;

  fermentadoresDisponibles: any[] = [];
  editandoFermentador: boolean = false;
  nuevoFermentadorId: number | string = '';
  guardandoFermentador: boolean = false;
  usuarioActual: any = null;
  recetas: Receta[] = [];

  ArrowLeft = ArrowLeft;
  Edit2 = Edit2;
  Check = Check;
  X = X;

  estadosMapping: { [key: number]: { nombre: string, color: string } } = {
    0: { nombre: 'Cancelado',   color: 'bg-red-100 text-red-800 border-red-200' },
    1: { nombre: 'Planificado', color: 'bg-blue-100 text-blue-800 border-blue-200' },
    2: { nombre: 'En Proceso',  color: 'bg-yellow-100 text-yellow-800 border-yellow-200' },
    3: { nombre: 'Finalizado',  color: 'bg-green-100 text-green-800 border-green-200' }
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private loteService: LoteService,
    private fermentadorService: FermentadorService,
    private authService: AuthService,
    private recetaService: RecetaService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      const idNumerico = Number(id);
      this.cargarLote(idNumerico);
      this.cargarInsumos(idNumerico);
      this.usuarioActual = this.authService.getCurrentUser();
      this.recetaService.getAll().subscribe({
      next: (data) => this.recetas = data ?? []
      });
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
      error: () => {
        this.error = 'Error al cargar el lote.';
        this.cargando = false;
      }
    });
  }

  cargarInsumos(id: number) {
    this.cargandoInsumos = true;
    this.loteService.getInsumosCombinados(id).subscribe({
      next: (data) => {
        this.insumos = data;
        this.cargandoInsumos = false;
      },
      error: () => { this.cargandoInsumos = false; }
    });
  }

  cargarFermentadores() {
    this.fermentadorService.getFermentadores().subscribe({
      next: (data) => this.fermentadoresDisponibles = data,
      error: (err) => console.error('Error al cargar fermentadores', err)
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
        this.cargarLote(this.lote!.id);
        this.editandoFermentador = false;
        this.guardandoFermentador = false;
      },
      error: () => {
        alert('Error al vincular fermentador.');
        this.editandoFermentador = false;
        this.guardandoFermentador = false;
      }
    });
  }

  volver() {
    this.router.navigate(['/planificacion/Listado']);
  }
  
  getNombreReceta(recetaId: number): string {
    const receta = this.recetas.find(r => r.idReceta === recetaId);
    return receta ? receta.nombre : `Receta #${recetaId}`;
  }
}