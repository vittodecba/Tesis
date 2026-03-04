import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Fermentador } from '../../Interfaces/fermentador';
import { FermentadorService } from '../../services/fermentador';
import { LucideAngularModule, Plus, Pencil, Trash2, Search } from 'lucide-angular'; // <-- NUEVOS ICONOS

@Component({
  selector: 'app-fermentador',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule], // <-- AGREGADO LUCIDE
  templateUrl: './fermentador.html',
  styleUrls: ['./fermentador.css']
})
export class FermentadorComponent implements OnInit {
  
  // Iconos para el HTML
  readonly Plus = Plus;
  readonly Pencil = Pencil;
  readonly Trash2 = Trash2;
  readonly Search = Search;

  listaFermentadores: Fermentador[] = [];
  mostrarModal: boolean = false; // <-- VARIABLE PARA EL MODAL
  
  nuevoFermentador: Fermentador = {
    nombre: '',
    capacidad: 0,
    estado: 'Disponible'
  };

  constructor(private _fermentadorService: FermentadorService) { }

  ngOnInit(): void {
    this.obtenerFermentadores();
  }

  // MÉTODOS PARA EL MODAL
  abrirModal() { this.mostrarModal = true; }
  cerrarModal() { 
    this.mostrarModal = false; 
    this.limpiarFormulario();
  }

  obtenerFermentadores() {
    this._fermentadorService.getFermentadores().subscribe(data => {
      this.listaFermentadores = data;
    }, error => {
      console.log(error);
    });
  }

  guardarFermentador() {
    if(this.nuevoFermentador.nombre === '' || this.nuevoFermentador.capacidad <= 0) {
        alert('Completar nombre y capacidad');
        return;
    }

    this._fermentadorService.crearFermentador(this.nuevoFermentador).subscribe(data => {
      this.obtenerFermentadores(); 
      this.cerrarModal(); // Cerramos el modal al terminar
    }, error => {
      console.log(error);
    });
  }

  limpiarFormulario() {
    this.nuevoFermentador = {
      nombre: '',
      capacidad: 0,
      estado: 'Disponible'
    };
  }
}