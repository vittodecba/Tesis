import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Pencil, Trash2, Plus, Search } from 'lucide-angular';
import { InsumoService } from '../../services/insumo.service';
import { UnidadMedidaService } from '../../services/unidadMedida';

@Component({
  selector: 'app-insumo',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './insumo.html',
  styleUrls: ['./insumo.css']
})
export class InsumoComponent implements OnInit {
  readonly Pencil = Pencil;
  readonly Trash2 = Trash2;
  readonly Plus = Plus;
  readonly Search = Search;

  insumos: any[] = [];          
  insumosOriginales: any[] = []; 
  mostrarModal: boolean = false;  
  mensajeError: string | null = null;
  tiposOpciones: any[] = []; 
  unidadesOpciones: any[] = [];
  unidadesFiltradas: any[] = []; 
  ajuste: boolean = false;
  paginaActual: number = 1;
  itemsPorPagina: number = 10;

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

  // --- GESTIÓN DE TIPOS (Existente) ---
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

  // --- GESTIÓN DE UNIDADES (Nueva integración) ---
 cargarUnidades() {
  this.unidadService.getUnidades().subscribe({
    next: (data: any) => {
      this.unidadesOpciones = data;
      const unidadesAceptables = ['Kg', 'Gr', 'Lt', 'Ml', 'Un'];
      
      // Filtramos y guardamos en nuestra nueva variable
      this.unidadesFiltradas = data.filter((u: any) => 
        unidadesAceptables.includes(u.abreviatura)
      );
    },
    error: (err: any) => console.error('Error al cargar unidades:', err)
  });
}

  crearUnidad() {
    const nombre = prompt("Nombre de la unidad (ej: Kilogramos):");
    if (!nombre) return;
    const abreviatura = prompt("Abreviatura (ej: Kg):");
    if (!abreviatura) return;

    const nuevaUnidad = { nombre: nombre.trim(), abreviatura: abreviatura.trim() };
    this.unidadService.crear(nuevaUnidad).subscribe({
      next: () => {
        alert('Unidad creada con éxito');
        this.unidadService.getUnidades().subscribe(data => {
          this.unidadesOpciones = data;
          const creada = data.find((u: any) => u.nombre === nuevaUnidad.nombre);
          if (creada) { this.datosForm.unidadMedidaId = creada.id; }
        });
      },
      error: (err) => alert('Error al crear unidad')
    });
  }

  eliminarUnidadDeLista() {
    const idABorrar = this.datosForm.unidadMedidaId;
    if (!idABorrar) {
      alert('Seleccioná una unidad en el desplegable para eliminarla');
      return;
    }
    const unidad = this.unidadesOpciones.find(u => u.id == idABorrar);
    if (confirm(`¿Estás seguro de eliminar "${unidad.nombre}"?`)) {
      this.unidadService.eliminar(idABorrar).subscribe({
        next: () => {
          alert('Unidad eliminada');
          this.datosForm.unidadMedidaId = null;
          this.cargarUnidades();
        },
        error: (err) => alert('No se puede eliminar: está siendo usada por insumos')
      });
    }
  }

  // --- LÓGICA DE INSUMOS ---
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

  abrirModal() { this.limpiarFormulario();  this.ajuste = false;this.mostrarModal = true; }
  cerrarModal() { this.mostrarModal = false; }
  
  limpiarFormulario() {
    this.datosForm = { id: null, codigo: '', nombreInsumo: '', tipoInsumoId: null, unidadMedidaId: null, stockActual: 0, observaciones: '' };
  }
  //Para editar lo que ya esta
  prepararEdicion(item: any) { 
    this.datosForm = { ...item };
    this.ajuste = false;
    this.mostrarModal = true; 
  }
  //Para agregar, o sea sumar  
  prepararAjuste(item: any) {
    this.datosForm = { ...item, stockActual: 0 };
    this.ajuste = true;
    this.mostrarModal = true;
  }

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
    this.insumoService.actualizarInsumo(payload.id, payload, this.ajuste).subscribe({
      next: () => {
        this.finalizarOperacion(this.ajuste ? 'Stock actualizado con éxito' : 'Insumo editado con éxito');
        this.ajuste = false;
      },
      error: (err) => {
        const mensaje = err.error?.message || err.error || 'Error al actualizar';
        alert(mensaje); 
      }
    });
  } else {   
    this.insumoService.crearInsumo(payload).subscribe({
      next: () => this.finalizarOperacion('Insumo creado con éxito'),
      error: (err) => {
        this.mensajeError = err.error?.message || err.error || 'Ya existe un insumo con ese nombre.';
        setTimeout(() => this.mensajeError = null, 5000);
      }
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