import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Fermentador } from '../../Interfaces/fermentador';
import { FermentadorService } from '../../services/fermentador';
import { LucideAngularModule, Plus, Pencil, Trash2, LineChart } from 'lucide-angular';

@Component({
  selector: 'app-fermentador',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './fermentador.html',
})
export class FermentadorComponent implements OnInit {
  readonly Plus = Plus;
  readonly Pencil = Pencil;
  readonly Trash2 = Trash2;
  readonly LineChart = LineChart;

  listaFermentadores: Fermentador[] = [];
  fermentadoresFiltrados: Fermentador[] = [];

  mostrarModal = false;
  esEdicion = false;
  idFermentadorEditar?: number;

  filtroCapacidad: number | null = null;
  filtroEstado: string = 'Todos';

  nuevoFermentador: Fermentador = this.crearFermentadorVacio();

  constructor(
    private _fermentadorService: FermentadorService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.obtenerFermentadores();
  }

  crearFermentadorVacio(): Fermentador {
    return {
      nombre: '',
      capacidad: 0,
      estado: '1',
      observaciones: '',
    };
  }

  normalizarEstado(estado: string | null | undefined): string {
    const mapa: Record<string, string> = {
      '1': '1',
      '2': '2',
      '3': '3',
      '4': '4',
      Disponible: '1',
      Ocupado: '2',
      Sucio: '3',
      Mantenimiento: '4',
    };

    return mapa[estado ?? ''] ?? '1';
  }

  obtenerTextoEstado(estado: string): string {
    const estadoNormalizado = this.normalizarEstado(estado);

    const mapa: Record<string, string> = {
      '1': 'Disponible',
      '2': 'Ocupado',
      '3': 'Sucio',
      '4': 'Mantenimiento',
    };

    return mapa[estadoNormalizado] ?? 'Desconocido';
  }

  obtenerClaseEstado(estado: string): string {
    const estadoNormalizado = this.normalizarEstado(estado);

    const mapa: Record<string, string> = {
      '1': 'bg-green-100 text-green-700',
      '2': 'bg-blue-100 text-blue-700',
      '3': 'bg-yellow-100 text-yellow-700',
      '4': 'bg-red-100 text-red-700',
    };

    return mapa[estadoNormalizado] ?? 'bg-gray-100 text-gray-700';
  }

  obtenerFermentadores() {
    this._fermentadorService.getFermentadores().subscribe({
      next: (data) => {
        this.listaFermentadores = data.map((f) => ({
          ...f,
          estado: this.normalizarEstado(f.estado),
        }));
        this.aplicarFiltros();
      },
      error: (err) => console.error('Error al cargar lista:', err),
    });
  }

  aplicarFiltros() {
    let res = [...this.listaFermentadores];

    if (this.filtroEstado !== 'Todos') {
      res = res.filter((f) => f.estado === this.filtroEstado);
    }

    const capacidadMinima = this.filtroCapacidad;
    if (capacidadMinima !== null) {
      res = res.filter((f) => f.capacidad >= capacidadMinima);
    }

    this.fermentadoresFiltrados = res;
  }

  abrirModal(item?: Fermentador) {
    if (item) {
      this.esEdicion = true;
      this.idFermentadorEditar = item.id;
      this.nuevoFermentador = {
        ...item,
        estado: this.normalizarEstado(item.estado),
      };
    } else {
      this.esEdicion = false;
      this.idFermentadorEditar = undefined;
      this.nuevoFermentador = this.crearFermentadorVacio();
    }

    this.mostrarModal = true;
  }

  cerrarModal() {
    this.mostrarModal = false;
  }

  guardarFermentador() {
    const dto = {
      nombre: this.nuevoFermentador.nombre,
      capacidad: this.nuevoFermentador.capacidad,
      estado: this.nuevoFermentador.estado,
      observaciones: this.nuevoFermentador.observaciones,
    };

    if (this.esEdicion && this.idFermentadorEditar) {
      this._fermentadorService.actualizarFermentador(this.idFermentadorEditar, dto).subscribe({
        next: () => {
          this.obtenerFermentadores();
          this.cerrarModal();
        },
        error: (err) => {
          console.error('Error update:', err);
          alert(err?.error || 'No se pudo actualizar el fermentador.');
        },
      });
    } else {
      this._fermentadorService.crearFermentador(dto).subscribe({
        next: () => {
          this.obtenerFermentadores();
          this.cerrarModal();
        },
        error: (err) => {
          console.error('Error create:', err);
          alert(err?.error || 'No se pudo crear el fermentador.');
        },
      });
    }
  }

  verGraficos(item: Fermentador) {
    if (!item.id) return;
    this.router.navigate(['/fermentadores', item.id]);
  }

  puedeEditarEstado(): boolean {
    return (
      !this.nuevoFermentador.loteId && this.normalizarEstado(this.nuevoFermentador.estado) !== '2'
    );
  }
}
