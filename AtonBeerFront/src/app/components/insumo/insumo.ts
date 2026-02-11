import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InsumoService } from '../../services/insumo.service';
import { UnidadMedidaService } from '../../services/unidadMedida'; // <--- AGREGADO

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
  
  tiposOpciones = [
    { id: 1, nombre: 'Maltas' },
    { id: 2, nombre: 'Lúpulos' },
    { id: 3, nombre: 'Levaduras' },
    { id: 4, nombre: 'Adjuntos' }
  ];

  // CAMBIO: Al tener la clase unidadMedida ahora es un array vacío que se llenará desde el Service
  unidadesOpciones: any[] = []; 

  filtroTexto: string = '';
  filtroTipo: string = ''; 
  orden: string = 'nombre';

  datosForm: any = {
    id: null,
    codigo: '',
    nombreInsumo: '',
    tipoInsumoId: null, 
    unidad: '',
    stockActual: 0,
    observaciones: ''
  };

  // Inyecto el nuevo servicio
  constructor(
    private insumoService: InsumoService,
    private unidadService: UnidadMedidaService // <--- AGREGADO
  ) {}

  ngOnInit(): void {
    this.cargarInsumos();
    this.cargarUnidades(); // <--- AGREGADO
  }

  // NUEVO MÉTODO: Trae las unidades de la DB
  cargarUnidades() {
    this.unidadService.getUnidades().subscribe({
      next: (data) => {
        this.unidadesOpciones = data;
      },
      error: (err) => console.error('Error al cargar unidades:', err)
    });
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
      resultado = resultado.filter(i => i.tipoInsumoId == this.filtroTipo);
    }

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
    this.datosForm = { 
      id: null, 
      codigo: '', 
      nombreInsumo: '', 
      tipoInsumoId: null, 
      unidad: '', 
      stockActual: 0, 
      observaciones: '' 
    };
  }

  prepararEdicion(item: any) {
    this.datosForm = { ...item };
    this.mostrarModal = true;
  }

  guardar() {
    if (!this.datosForm.nombreInsumo || !this.datosForm.tipoInsumoId) {
      alert('Nombre y Tipo son obligatorios');
      return;
    }
    const payload = {
      ...this.datosForm,
      tipoInsumoId: Number(this.datosForm.tipoInsumoId)
    };

    if (!this.datosForm.id) {
      const timestamp = new Date().getTime().toString().slice(-4);
      this.datosForm.codigo = "INS-" + timestamp;
    }

    if (this.datosForm.id) {
      this.insumoService.actualizarInsumo(this.datosForm.id, payload).subscribe({
        next: () => this.finalizarOperacion('Insumo actualizado'),
        error: (err) => alert("Error: " + (err.error?.message || "Error al actualizar"))
      });
    } else {
      this.insumoService.crearInsumo(payload).subscribe({
        next: () => this.finalizarOperacion('Insumo creado'),
        error: (err) => alert("Error: " + (err.error?.message || "Error al crear"))
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