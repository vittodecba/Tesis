import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InsumoService } from '../../services/insumo.service';
import { UnidadMedidaService } from '../../services/unidadMedida';

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
    const nuevoObjeto = {
      nombre: nombre,
      activo: true
    };

    this.insumoService.crearTipo(nuevoObjeto).subscribe({
      next: (res: any) => {
        alert('Categoría creada con éxito');
        // Recargamos la lista desde el Back
        this.insumoService.obtenerTipos().subscribe(data => {
          this.tiposOpciones = data;       
        
          const creado = data.find((t: any) => t.nombre === nombre);
          if (creado) {
            this.datosForm.tipoInsumoId = creado.id;
          }
        });
      },
      error: (err) => alert('Error al crear: ' + err.error)
    });
  }
}

  cargarTipos() {
    this.insumoService.obtenerTipos().subscribe({
      next: (data: any) => {
        console.log('DATOS DE TIPOS RECIBIDOS:', data); // <-- REVISÁ ESTO EN LA CONSOLA F12
        this.tiposOpciones = data;
      },
      error: (err: any) => console.error('Error al cargar tipos:', err)
    });
  }
eliminarTipoDeLista() {
  // 1. Buscamos el ID de lo que está seleccionado
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
        this.datosForm.tipoInsumoId = null; // Limpiamos la selección
        this.cargarTipos(); // Refrescamos la lista para que desaparezca del combo
      },
      error: (err) => {
        // Tu Backend ya tiene la validación: si hay insumos usándolo, no te deja
        alert('No se puede eliminar: ' + (err.error?.message || 'La categoría está en uso'));
      }
    });
  }
}
  cargarUnidades() {
    this.unidadService.getUnidades().subscribe({
      next: (data: any) => {
        this.unidadesOpciones = data;
      },
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

    if (this.filtroTipo) {
      // Usamos doble igual para permitir comparación de string con number
      resultado = resultado.filter(i => i.tipoInsumoId == this.filtroTipo);
    }

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
  
  prepararEdicion(item: any) { 
    this.datosForm = { ...item }; 
    this.mostrarModal = true; 
  }

  guardar() {
    if (!this.datosForm.nombreInsumo || !this.datosForm.tipoInsumoId || !this.datosForm.unidadMedidaId) {
      alert('Nombre, Tipo y Unidad son obligatorios');
      return;
    }

    const payload = {
      ...this.datosForm,
      tipoInsumoId: Number(this.datosForm.tipoInsumoId),
      unidadMedidaId: Number(this.datosForm.unidadMedidaId)
    };

    if (this.datosForm.id) {
      this.insumoService.actualizarInsumo(this.datosForm.id, payload).subscribe({
        next: () => this.finalizarOperacion('Insumo actualizado'),
        error: (err: any) => alert("Error: " + (err.error?.message || "Error al actualizar"))
      });
    } else {
      this.insumoService.crearInsumo(payload).subscribe({
        next: () => this.finalizarOperacion('Insumo creado'),
        error: (err: any) => alert("Error: " + (err.error?.message || "Error al crear"))
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