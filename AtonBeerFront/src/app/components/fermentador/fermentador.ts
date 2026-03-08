import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Fermentador } from '../../Interfaces/fermentador';
import { FermentadorService } from '../../services/fermentador';
import { LucideAngularModule, Plus, Pencil, Trash2 } from 'lucide-angular';

@Component({
  selector: 'app-fermentador',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './fermentador.html',
  styleUrls: ['./fermentador.css'],
})
export class FermentadorComponent implements OnInit {
  readonly Plus = Plus;
  readonly Pencil = Pencil;
  readonly Trash2 = Trash2;

  listaFermentadores: Fermentador[] = [];
  mostrarModal: boolean = false;
  esEdicion: boolean = false;
  idFermentadorEditar?: number;

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
    this._fermentadorService.getFermentadores().subscribe((data) => {
      this.listaFermentadores = data;
    });
  }

  abrirModal(item?: Fermentador) {
    this.mostrarModal = true;
    if (item) {
      this.esEdicion = true;
      this.idFermentadorEditar = item.id;
      this.nuevoFermentador = { ...item };

      // FORZAR SINCRONIZACIÓN:
      // Si el item viene con texto, lo pasamos a número para el <select>
      const mapeo: any = { Disponible: 1, Ocupado: 2, Sucio: 3, Mantenimiento: 4 };

      if (typeof item.estado === 'string') {
        this.nuevoFermentador.estado = mapeo[item.estado] || 1;
      }
    } else {
      this.esEdicion = false;
      this.limpiarFormulario();
    }
  }

  cerrarModal() {
    this.mostrarModal = false;
    this.limpiarFormulario();
  }

  guardarFermentador() {
    const dto = {
      nombre: this.nuevoFermentador.nombre,
      capacidad: this.nuevoFermentador.capacidad,
      estado: Number(this.nuevoFermentador.estado), // Enviamos siempre el número (1-4)
      observaciones: this.nuevoFermentador.observaciones,
    };

    if (this.esEdicion && this.idFermentadorEditar) {
      this._fermentadorService
        .actualizarFermentador(this.idFermentadorEditar, dto)
        .subscribe(() => {
          this.obtenerFermentadores();
          this.cerrarModal();
        });
    } else {
      this._fermentadorService.crearFermentador(this.nuevoFermentador).subscribe(() => {
        this.obtenerFermentadores();
        this.cerrarModal();
      });
    }
  }

  limpiarFormulario() {
    this.nuevoFermentador = { nombre: '', capacidad: 0, estado: 1, observaciones: '' };
    this.esEdicion = false;
    this.idFermentadorEditar = undefined;
  }
}
