import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Fermentador } from '../../Interfaces/fermentador';
import { FermentadorService } from '../../services/fermentador';
import { LucideAngularModule, Plus, Pencil, Trash2, ArrowUpDown } from 'lucide-angular';

@Component({
  selector: 'app-fermentador',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './fermentador.html',
})
export class FermentadorComponent implements OnInit {
  // Definición de iconos para que Lucide los encuentre
  readonly Plus = Plus;
  readonly Pencil = Pencil;
  readonly Trash2 = Trash2;
  readonly ArrowUpDown = ArrowUpDown;

  listaFermentadores: Fermentador[] = [];
  fermentadoresFiltrados: Fermentador[] = [];

  mostrarModal: boolean = false;
  esEdicion: boolean = false;
  idFermentadorEditar?: number;
  filtroCapacidad: number | null = null;

  filtroEstado: string = 'Todos';
  ordenCapacidad: string = 'Todos';

  nuevoFermentador: Fermentador = {
    nombre: '',
    capacidad: 0,
    estado: 1,
    observaciones: '',
  };

  constructor(private _fermentadorService: FermentadorService) {}

  ngOnInit(): void {
    this.obtenerFermentadores();
  }

  obtenerFermentadores() {
    this._fermentadorService.getFermentadores().subscribe({
      next: (data) => {
        this.listaFermentadores = data;
        this.aplicarFiltros();
      },
      error: (err) => console.error('Error al cargar lista:', err),
    });
  }

  // Dentro de la clase...

  aplicarFiltros() {
    let res = [...this.listaFermentadores];

    // Filtro de estado
    if (this.filtroEstado !== 'Todos') {
      res = res.filter((f) => f.estado === this.filtroEstado);
    }

    // Filtro de capacidad mínima
    if (this.filtroCapacidad !== null) {
      res = res.filter((f) => f.capacidad >= this.filtroCapacidad!);
    }

    this.fermentadoresFiltrados = res;
  }

  // ESTA FUNCIÓN ES LA CLAVE DE LOS BOTONES
  abrirModal(item?: Fermentador) {
    if (item) {
      this.esEdicion = true;
      this.idFermentadorEditar = item.id;
      // Creamos una copia para no editar la lista directamente antes de guardar
      this.nuevoFermentador = JSON.parse(JSON.stringify(item));

      // Convertimos el estado texto a número para el <select> del modal
      const mapeo: any = { Disponible: 1, Ocupado: 2, Sucio: 3, Mantenimiento: 4 };
      if (isNaN(Number(this.nuevoFermentador.estado))) {
        this.nuevoFermentador.estado = mapeo[this.nuevoFermentador.estado] || 1;
      }
    } else {
      this.esEdicion = false;
      this.idFermentadorEditar = undefined;
      this.nuevoFermentador = { nombre: '', capacidad: 0, estado: 1, observaciones: '' };
    }
    this.mostrarModal = true;
  }

  cerrarModal() {
    this.mostrarModal = false;
  }

  guardarFermentador() {
    const dto = {
      ...this.nuevoFermentador,
      estado: Number(this.nuevoFermentador.estado),
    };

    if (this.esEdicion && this.idFermentadorEditar) {
      this._fermentadorService.actualizarFermentador(this.idFermentadorEditar, dto).subscribe({
        next: () => {
          this.obtenerFermentadores();
          this.cerrarModal();
        },
        error: (err) => console.error('Error update:', err),
      });
    } else {
      this._fermentadorService.crearFermentador(dto).subscribe({
        next: () => {
          this.obtenerFermentadores();
          this.cerrarModal();
        },
        error: (err) => console.error('Error create:', err),
      });
    }
  }
}
