import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
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

  mostrarModal: boolean = false;
  esEdicion: boolean = false;
  idFermentadorEditar?: number;
  filtroCapacidad: number | null = null;
  filtroEstado: string = 'Todos';

  // CORRECCIÓN: Inicializado con string '1' para evitar TS2322
  nuevoFermentador: Fermentador = {
    nombre: '',
    capacidad: 0,
    estado: '1',
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

  aplicarFiltros() {
    let res = [...this.listaFermentadores];
    if (this.filtroEstado !== 'Todos') {
      res = res.filter((f) => f.estado === this.filtroEstado);
    }
    if (this.filtroCapacidad !== null) {
      res = res.filter((f) => f.capacidad >= this.filtroCapacidad!);
    }
    this.fermentadoresFiltrados = res;
  }

  abrirModal(item?: Fermentador) {
    if (item) {
      this.esEdicion = true;
      this.idFermentadorEditar = item.id;
      this.nuevoFermentador = JSON.parse(JSON.stringify(item));

      // CORRECCIÓN: Aseguramos que el estado sea un string ID ('1', '2', etc)
      // Si el backend mandó el nombre del estado, lo mapeamos a su ID en string
      const mapeoTextoAID: any = {
        Disponible: '1',
        Ocupado: '2',
        Sucio: '3',
        Mantenimiento: '4',
      };

      if (mapeoTextoAID[this.nuevoFermentador.estado]) {
        this.nuevoFermentador.estado = mapeoTextoAID[this.nuevoFermentador.estado];
      }
    } else {
      this.esEdicion = false;
      this.idFermentadorEditar = undefined;
      this.nuevoFermentador = { nombre: '', capacidad: 0, estado: '1', observaciones: '' };
    }
    this.mostrarModal = true;
  }

  cerrarModal() {
    this.mostrarModal = false;
  }

  guardarFermentador() {
    // CORRECCIÓN: Mantener estado como string para cumplir con la interfaz Fermentador
    const dto = this.nuevoFermentador;

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

  verGraficos(item: Fermentador) {
    console.log('Ver mediciones para lote:', item.loteId);
  }
}
