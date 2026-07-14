import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Pencil, Trash2, Plus, Search, ChevronLeft, ChevronRight } from 'lucide-angular';
import { InsumoService } from '../../services/insumo.service';
import { UnidadMedidaService } from '../../services/unidadMedida';
import { NotificationService } from '../../core/services/notification.service';

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
  readonly ChevronLeft = ChevronLeft;
  readonly ChevronRight = ChevronRight;

  insumos: any[] = [];
  insumosOriginales: any[] = [];
  mostrarModal: boolean = false;
  mostrarModalEliminar: boolean = false;
  insumoAEliminar: any = null;
  recetasAfectadas: any[] = [];
  cargandoRecetas: boolean = false;
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
    private unidadService: UnidadMedidaService,
    private noti: NotificationService
  ) {}

  ngOnInit(): void {
    this.cargarInsumos();
    this.cargarUnidades();
    this.cargarTipos();
  }

  async crearTipos(objetivo: string) {
    const nombre = await this.noti.prompt({ titulo: `Nueva categoría de ${objetivo}`, placeholder: 'Nombre de la categoría' });
    if (nombre && nombre.trim() !== '') {
      const nuevoObjeto = { nombre: nombre, activo: true };
      this.insumoService.crearTipo(nuevoObjeto).subscribe({
        next: (res: any) => {
          this.noti.success('Categoría creada con éxito');
          this.insumoService.obtenerTipos().subscribe(data => {
            this.tiposOpciones = data;
            const creado = data.find((t: any) => t.nombre === nombre);
            if (creado) { this.datosForm.tipoInsumoId = creado.id; }
          });
        },
        error: (err) => this.noti.error('Error al crear: ' + err.error)
      });
    }
  }

  cargarTipos() {
    this.insumoService.obtenerTipos().subscribe({
      next: (data: any) => { this.tiposOpciones = data; },
      error: (err: any) => console.error('Error al cargar tipos:', err)
    });
  }

  async eliminarTipoDeLista() {
    const idABorrar = this.datosForm.tipoInsumoId;
    if (!idABorrar) {
      this.noti.warning('Primero seleccioná en el desplegable qué tipo querés eliminar');
      return;
    }
    const tipoCualquiera = this.tiposOpciones.find(t => t.id == idABorrar);
    const ok = await this.noti.confirm({
      titulo: '¿Eliminar categoría?',
      texto: `Se eliminará la categoría "${tipoCualquiera.nombre}".`,
      peligro: true
    });
    if (ok) {
      this.insumoService.eliminarTipo(idABorrar).subscribe({
        next: () => {
          this.noti.success('Categoría eliminada con éxito');
          this.datosForm.tipoInsumoId = null;
          this.cargarTipos();
        },
        error: (err) => this.noti.error('No se puede eliminar: ' + (err.error?.message || 'La categoría está en uso'))
      });
    }
  }

  cargarUnidades() {
    this.unidadService.getUnidades().subscribe({
      next: (data: any) => {
        this.unidadesOpciones = data;
        this.unidadesFiltradas = data.filter((u: any) => u.activo);
      },
      error: (err: any) => console.error('Error al cargar unidades:', err)
    });
  }

  async crearUnidad() {
    const nombre = await this.noti.prompt({ titulo: 'Nueva unidad', texto: 'Nombre de la unidad', placeholder: 'ej: Kilogramos' });
    if (!nombre) return;
    const abreviatura = await this.noti.prompt({ titulo: 'Nueva unidad', texto: 'Abreviatura', placeholder: 'ej: Kg' });
    if (!abreviatura) return;

    const nuevaUnidad = { nombre: nombre.trim(), abreviatura: abreviatura.trim() };
    this.unidadService.crear(nuevaUnidad).subscribe({
      next: () => {
        this.noti.success('Unidad creada con éxito');
        this.unidadService.getUnidades().subscribe(data => {
          this.unidadesOpciones = data;
          const creada = data.find((u: any) => u.nombre === nuevaUnidad.nombre);
          if (creada) { this.datosForm.unidadMedidaId = creada.id; }
        });
      },
      error: (err) => this.noti.error('Error al crear unidad')
    });
  }

  async eliminarUnidadDeLista() {
    const idABorrar = this.datosForm.unidadMedidaId;
    if (!idABorrar) {
      this.noti.warning('Seleccioná una unidad en el desplegable para eliminarla');
      return;
    }
    const unidad = this.unidadesOpciones.find(u => u.id == idABorrar);
    const ok = await this.noti.confirm({
      titulo: '¿Eliminar unidad?',
      texto: `Se eliminará la unidad "${unidad.nombre}".`,
      peligro: true
    });
    if (ok) {
      this.unidadService.eliminar(idABorrar).subscribe({
        next: () => {
          this.noti.success('Unidad eliminada');
          this.datosForm.unidadMedidaId = null;
          this.cargarUnidades();
        },
        error: (err) => this.noti.error('No se puede eliminar: está siendo usada por insumos')
      });
    }
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
    this.paginaActual = 1;
  }

  get insumosPaginados() {
    const listaAVisualizar = this.insumos || [];
    const inicio = (this.paginaActual - 1) * this.itemsPorPagina;
    const fin = inicio + this.itemsPorPagina;
    
    return listaAVisualizar.slice(inicio, fin);
  }

  abrirModal() { this.limpiarFormulario();  this.ajuste = false;this.mostrarModal = true; }
  cerrarModal() { this.mostrarModal = false; }

  limpiarFormulario() {
    this.datosForm = { id: null, codigo: '', nombreInsumo: '', tipoInsumoId: null, unidadMedidaId: null, stockActual: 0, observaciones: '' };
  }

  prepararEdicion(item: any) {
    this.datosForm = { ...item };
    this.ajuste = false;
    this.mostrarModal = true;
  }

  prepararAjuste(item: any) {
    this.datosForm = { ...item, stockActual: 0 };
    this.ajuste = true;
    this.mostrarModal = true;
  }

  guardar() {
    if (!this.datosForm.nombreInsumo || !this.datosForm.tipoInsumoId || !this.datosForm.unidadMedidaId) {
      this.noti.warning('Por favor, completá Nombre, Tipo y Unidad.');
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
          this.noti.error(mensaje);
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

  eliminar(item: any) {
    this.insumoAEliminar = item;
    this.recetasAfectadas = [];
    this.cargandoRecetas = true;
    this.mostrarModalEliminar = true;

    this.insumoService.getRecetasQueUsanInsumo(item.id).subscribe({
      next: (data: any[]) => {
        this.recetasAfectadas = data || [];
        this.cargandoRecetas = false;
      },
      error: () => {
        this.recetasAfectadas = [];
        this.cargandoRecetas = false;
      }
    });
  }

  confirmarEliminacion() {
    if (!this.insumoAEliminar) return;
    this.insumoService.eliminarInsumo(this.insumoAEliminar.id).subscribe({
      next: () => {
        this.cerrarModalEliminar();
        this.cargarInsumos();
      },
      error: (err: any) => this.noti.error('Error al eliminar')
    });
  }

  cerrarModalEliminar() {
    this.mostrarModalEliminar = false;
    this.insumoAEliminar = null;
    this.recetasAfectadas = [];
    this.cargandoRecetas = false;
  }

  finalizarOperacion(mensaje: string) {
    this.noti.success(mensaje);
    this.cerrarModal();
    this.cargarInsumos();
  }
}