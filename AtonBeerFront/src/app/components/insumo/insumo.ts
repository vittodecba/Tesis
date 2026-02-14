import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Pencil, Trash2, Plus, Search } from 'lucide-angular'; // Agregado Lucide
import { InsumoService } from '../../services/insumo.service';
import { UnidadMedidaService } from '../../services/unidadMedida';

@Component({
  selector: 'app-insumo',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule], // Agregado LucideAngularModule
  templateUrl: './insumo.html',
  styleUrls: ['./insumo.css']
})
export class InsumoComponent implements OnInit {
  // Íconos para el HTML
  readonly Pencil = Pencil;
  readonly Trash2 = Trash2;
  readonly Plus = Plus;
  readonly Search = Search;

  insumos: any[] = [];          
  insumosOriginales: any[] = []; 
  mostrarModal: boolean = false;  
  
  tiposOpciones: any[] = []; 
  unidadesOpciones: any[] = []; 

  filtroTexto: string = '';
  filtroTipo: string = ''; 
  orden: string = 'nombre';

  datosForm: any = {
    id: null,
    codigo: '',
    nombreInsumo: '',
    tipoInsumoId: null, 
    unidadMedidaId: null,
    stockActual: 0,
    observaciones: ''
  };

  constructor(
    private insumoService: InsumoService,
    private unidadService: UnidadMedidaService
  ) {}

  ngOnInit(): void {
    this.cargarInsumos();
    this.cargarUnidades();
    this.cargarTipos(); 
  }

  crearTipos(objetivo: string) {
    const nombre = prompt(`Ingrese el nombre de la nueva categoría de ${objetivo}:`);
    if (nombre && nombre.trim() !== '') {   
      const nuevoObjeto = { nombre: nombre, activo: true };
      this.insumoService.crearTipo(nuevoObjeto).subscribe({
        next: (res: any) => {
          alert('Categoría creada con éxito');
          this.insumoService.obtenerTipos().subscribe(data => {
            this.tiposOpciones = data;       
            const creado = data.find((t: any) => t.nombre === nombre);
            if (creado) { this.datosForm.tipoInsumoId = creado.id; }
          });
        },
        error: (err) => alert('Error al crear: ' + err.error)
      });
    }
  }

  cargarTipos() {
    this.insumoService.obtenerTipos().subscribe({
      next: (data: any) => { this.tiposOpciones = data; },
      error: (err: any) => console.error('Error al cargar tipos:', err)
    });
  }

  eliminarTipoDeLista() {
    const idABorrar = this.datosForm.tipoInsumoId;
    if (!idABorrar) {
      alert('Primero seleccioná en el desplegable qué tipo querés eliminar');
      return;
    }
    const tipoCualquiera = this.tiposOpciones.find(t => t.id == idABorrar);  
    if (confirm(`¿Estás seguro de que querés eliminar la categoría "${tipoCualquiera.nombre}"?`)) {
      this.insumoService.eliminarTipo(idABorrar).subscribe({
        next: () => {
          alert('Categoría eliminada con éxito');
          this.datosForm.tipoInsumoId = null;
          this.cargarTipos();
        },
        error: (err) => alert('No se puede eliminar: ' + (err.error?.message || 'La categoría está en uso'))
      });
    }
  }

  cargarUnidades() {
    this.unidadService.getUnidades().subscribe({
      next: (data: any) => { this.unidadesOpciones = data; },
      error: (err: any) => console.error('Error al cargar unidades:', err)
    });
  }

  cargarInsumos() {
    this.insumoService.obtenerInsumos().subscribe({
      next: (data: any) => {
        this.insumosOriginales = data;
        this.aplicarFiltros(); 
      },
      error: (err: any) => console.error('Error al cargar:', err)
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
    if (this.filtroTipo) { resultado = resultado.filter(i => i.tipoInsumoId == this.filtroTipo); }
    resultado.sort((a, b) => {
      if (this.orden === 'nombre') return a.nombreInsumo.localeCompare(b.nombreInsumo);
      if (this.orden === 'stock') return b.stockActual - a.stockActual;
      if (this.orden === 'fecha') {
        const fechaA = a.ultimaActualizacion ? new Date(a.ultimaActualizacion).getTime() : 0;
        const fechaB = b.ultimaActualizacion ? new Date(b.ultimaActualizacion).getTime() : 0;
        return fechaB - fechaA;
      }
      return 0;
    });
    this.insumos = resultado;
  }

  abrirModal() { this.limpiarFormulario(); this.mostrarModal = true; }
  cerrarModal() { this.mostrarModal = false; }
  
  limpiarFormulario() {
    this.datosForm = { id: null, codigo: '', nombreInsumo: '', tipoInsumoId: null, unidadMedidaId: null, stockActual: 0, observaciones: '' };
  }
  
  prepararEdicion(item: any) { this.datosForm = { ...item }; this.mostrarModal = true; }

  guardar() {
    if (!this.datosForm.nombreInsumo || !this.datosForm.tipoInsumoId || !this.datosForm.unidadMedidaId) {
      alert('Por favor, completá Nombre, Tipo y Unidad.');
      return;
    }
    const payload = {
      id: this.datosForm.id ? Number(this.datosForm.id) : 0, 
      nombreInsumo: this.datosForm.nombreInsumo,
      codigo: this.datosForm.codigo || "",
      tipoInsumoId: Number(this.datosForm.tipoInsumoId),
      unidadMedidaId: Number(this.datosForm.unidadMedidaId),
      stockActual: Number(this.datosForm.stockActual) || 0,
      observaciones: this.datosForm.observaciones || "",
      ultimaActualizacion: new Date().toISOString() 
    };

    if (payload.id > 0) {
      this.insumoService.actualizarInsumo(payload.id, payload).subscribe({
        next: () => this.finalizarOperacion('Insumo actualizado con éxito'),
        error: (err) => alert('Error al actualizar: ' + (err.error?.message || err.status))
      });
    } else {
      this.insumoService.crearInsumo(payload).subscribe({
        next: () => this.finalizarOperacion('Insumo creado con éxito'),
        error: (err) => alert('Error al crear: Verificá los datos ingresados.')
      });
    }
  }

  eliminar(id: number) {
    if (confirm('¿Estás seguro de eliminar este insumo?')) {
      this.insumoService.eliminarInsumo(id).subscribe({
        next: () => this.cargarInsumos(),
        error: (err: any) => alert('Error al eliminar')
      });
    }
  }

  finalizarOperacion(mensaje: string) {
    alert(mensaje);
    this.cerrarModal();
    this.cargarInsumos();
  }
}