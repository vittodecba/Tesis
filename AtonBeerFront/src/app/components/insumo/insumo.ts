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
  insumosOriginales: any[] = []; 

  mostrarModal: boolean = false;
  tiposOpciones: string[] = ['Malta', 'Lúpulo', 'Levadura', 'Limpieza', 'Envases', 'Químicos', 'Otro'];
  unidadesOpciones: string[] = ['Kg', 'Gr', 'L', 'Ml', 'Unidad'];

  filtroTexto: string = '';
  filtroTipo: string = '';
  orden: string = 'nombre';

  datosForm: any = {
    id: null,
    codigo: '',
    nombreInsumo: '',
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
      next: (data) => {
        this.insumosOriginales = data;
        this.aplicarFiltros(); 
      },
      error: (err) => console.error('Error al cargar:', err)
    });
  }

  aplicarFiltros() {
    let resultado = [...this.insumosOriginales];
    
    if (this.filtroTexto) {
      const busqueda = this.filtroTexto.toLowerCase();
      resultado = resultado.filter(i => 
        i.nombreInsumo.toLowerCase().includes(busqueda) ||
        (i.codigo && i.codigo.toLowerCase().includes(busqueda))
      );
    }

    if (this.filtroTipo) {
      resultado = resultado.filter(i => i.tipo === this.filtroTipo);
    }

    // Lógica de ordenamiento recuperada
    resultado.sort((a, b) => {
      if (this.orden === 'nombre') return a.nombreInsumo.localeCompare(b.nombreInsumo);
      if (this.orden === 'stock') return b.stockActual - a.stockActual;
      if (this.orden === 'fecha') {
        return new Date(b.ultimaActualizacion).getTime() - new Date(a.ultimaActualizacion).getTime();
      }
      return 0;
    });

    this.insumos = resultado;
  }

  abrirModal() {
    this.limpiarFormulario();
    this.mostrarModal = true;
  }

  cerrarModal() {
    this.mostrarModal = false;
  }

  limpiarFormulario() {
    this.datosForm = { id: null, codigo: '', nombreInsumo: '', tipo: '', unidad: '', stockActual: 0, observaciones: '' };
  }

  prepararEdicion(item: any) {
    this.datosForm = { ...item };
    this.mostrarModal = true;
  }

  guardar() {
    if (!this.datosForm.nombreInsumo || !this.datosForm.tipo) {
      alert('Nombre y Tipo son obligatorios');
      return;
    }

    if (!this.datosForm.id) {
      const timestamp = new Date().getTime().toString().slice(-4);
      this.datosForm.codigo = this.datosForm.tipo.substring(0, 3).toUpperCase() + "-" + timestamp;
    }

    if (this.datosForm.id) {
      this.insumoService.actualizarInsumo(this.datosForm.id, this.datosForm).subscribe({
        next: () => this.finalizarOperacion('Insumo actualizado'),
        error: (err) => alert("Error: " + (err.error?.message || JSON.stringify(err.error)))
      });
    } else {
      this.insumoService.crearInsumo(this.datosForm).subscribe({
        next: () => this.finalizarOperacion('Insumo creado'),
        error: (err) => alert("Error: " + (err.error?.message || JSON.stringify(err.error)))
      });
    }
  }

  eliminar(id: number) {
    if (confirm('¿Estás seguro de eliminar este insumo?')) {
      this.insumoService.eliminarInsumo(id).subscribe({
        next: () => this.cargarInsumos(),
        error: (err) => alert('Error al eliminar')
      });
    }
  }

  finalizarOperacion(mensaje: string) {
    alert(mensaje);
    this.cerrarModal();
    this.cargarInsumos();
  }
}