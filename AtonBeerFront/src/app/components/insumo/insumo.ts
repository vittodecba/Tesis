import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InsumoService } from '../../services/insumo.service';

@Component({
  selector: 'app-insumo',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './insumo.html',
  styleUrls: ['./insumo.css']
})
export class InsumoComponent implements OnInit {
  insumos: any[] = [];
  mostrarModal: boolean = false;
  
  tiposOpciones: string[] = ['Malta', 'Lúpulo', 'Levadura', 'Limpieza', 'Envases', 'Químicos', 'Otro'];
  unidadesOpciones: string[] = ['Kg', 'Gr', 'L', 'Ml', 'Unidad'];

  datosForm: any = {
    nombreInsumo: '',
    // codigo: '', // Ya no lo usamos, es automático
    tipo: '',
    unidad: '',
    stockActual: 0,
    observaciones: ''
  };

  constructor(private insumoService: InsumoService) {}

  ngOnInit(): void {
     this.cargarInsumos();
  }

  cargarInsumos() {
    this.insumoService.obtenerInsumos().subscribe({
      next: (data: any) => {
        this.insumos = data;
      },
      error: (err: any) => {
        console.error('Error al cargar:', err);
      }
    });
  }

  abrirModal() {
    this.mostrarModal = true;
    this.limpiarFormulario();
  }

  cerrarModal() {
    this.mostrarModal = false;
  }

  limpiarFormulario() {
    this.datosForm = {
      nombreInsumo: '',
      codigo: '',
      tipo: '',
      unidad: '',
      stockActual: 0,
      observaciones: ''
    };
  }

  guardar() {
    // Validación simple en el frente
    if (!this.datosForm.nombreInsumo || !this.datosForm.tipo) {
      alert('Por favor complete al menos el Nombre y el Tipo');
      return;
    }

    this.insumoService.crearInsumo(this.datosForm).subscribe({
      next: (resp) => {
        alert('Insumo creado con éxito!');
        this.cerrarModal();
        this.cargarInsumos();
      },
      error: (err) => {
        console.error(err);
        
        // --- CAMBIO ACÁ ---
        // Si el backend manda un mensaje (ej: "El insumo ya existe"), lo mostramos.
        // Si no, mostramos el genérico.
        if (err.error && typeof err.error === 'string') {
            alert(err.error); 
        } else {
            alert('Error al guardar. Verifique la consola.');
        }
        // ------------------
      }
    });
  }
}