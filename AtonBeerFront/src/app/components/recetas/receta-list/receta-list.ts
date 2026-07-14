import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import {
  LucideAngularModule,
  Search, Plus, Copy, Pencil, FileText, X, Filter, Beer, ChevronDown, ChevronLeft, ChevronRight,Trash2, Save
} from 'lucide-angular';
import { RecetaService, Receta } from '../../../services/receta';
import { InsumoService } from '../../../services/insumo.service';
import { UnidadMedidaService } from '../../../services/unidadMedida';
import { NotificationService } from '../../../core/services/notification.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-receta-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, LucideAngularModule, RouterModule],
  templateUrl: './receta-list.html',
  styleUrls: ['./receta-list.css']
})
export class RecetaListComponent implements OnInit {
  Search = Search; Plus = Plus; Pencil = Pencil; 
  FileText = FileText; X = X; Filter = Filter; 
  Beer = Beer; ChevronDown = ChevronDown; ChevronLeft = ChevronLeft; ChevronRight = ChevronRight; Trash2 = Trash2; Save = Save; Copy = Copy;

  recetas: Receta[] = [];
  recetasFiltradas: any[] = [];
  paginaActual: number = 1;
  itemsPorPagina: number = 8;
  cargando = false;
  showModal = false; 
  isEditing = false; 
  recetaIdSeleccionada: number | null = null; 
  form: FormGroup;
  mensajeError: string | null = null; 
  pasos: any[] = [];
  filtroNombre: string = '';
  filtroEstilo: string = '';
  filtroEstado: string = 'Activa';   
  estilos: string[] = [];
  estilosDefault: string[] = ['IPA', 'Stout', 'Golden', 'Honey'];
  
  listaInsumos: any[] = [];
  listaUnidades: any[] = []; // <--- NUEVA LISTA
  insumosElegidos: any[] = [];
  insumoIdSeleccionado: number = 0;
  unidadIdSeleccionada: number = 0; // <--- NUEVA VARIABLE
  cantidadIngresada: number = 0; 


  constructor(
    private recetaService: RecetaService, 
    private fb: FormBuilder, 
    private insumoService: InsumoService,
    private unidadService: UnidadMedidaService, // <--- INYECTADO
    private router : Router,
    private noti: NotificationService
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required]],
      estilo: ['', [Validators.required]],
      batchSizeLitros: [20, [Validators.required, Validators.min(1)]],
      notas: [''],
      estado: ['Activa']
    });
  }

  ngOnInit(): void {
    this.cargarEstilosLocales(); 
    this.loadRecetas();
    this.cargarInsumos();
    this.cargarUnidades(); // <--- LLAMADA INICIAL
  }

  cargarUnidades(): void {
    this.unidadService.getUnidades().subscribe({
      next: (data) => this.listaUnidades = data
    });
  }

  cargarEstilosLocales() {
    const guardados = localStorage.getItem('estilos_cerveza');
    if (guardados) {
      this.estilos = JSON.parse(guardados);
    } else {
      this.estilos = [...this.estilosDefault];
      this.guardarEstilosLocales();
    }
  }

  guardarEstilosLocales() {
    localStorage.setItem('estilos_cerveza', JSON.stringify(this.estilos));
  }

  async agregarEstilo() {
    const nuevoEstilo = await this.noti.prompt({ titulo: 'Nuevo estilo de cerveza', placeholder: 'ej: IPA' });
    if (nuevoEstilo && nuevoEstilo.trim() !== '') {
      const estiloLimpio = nuevoEstilo.trim();
      if (!this.estilos.includes(estiloLimpio)) {
        this.estilos.push(estiloLimpio);
        this.guardarEstilosLocales();
        this.form.patchValue({ estilo: estiloLimpio });
      }
    }
  }

  async eliminarEstilo() {
    const estiloSeleccionado = this.form.get('estilo')?.value;
    if (!estiloSeleccionado) return;
    const ok = await this.noti.confirm({ titulo: '¿Eliminar estilo?', texto: `Se eliminará "${estiloSeleccionado}".`, peligro: true });
    if (ok) {
      this.estilos = this.estilos.filter(e => e !== estiloSeleccionado);
      this.guardarEstilosLocales();
      this.form.patchValue({ estilo: '' });
    }
  }

  cargarInsumos(): void {
    this.insumoService.obtenerInsumos().subscribe({
      next: (data) => this.listaInsumos = data
    });
  }
  agregarInsumoALista() {
    if (this.insumoIdSeleccionado > 0 && this.cantidadIngresada > 0 && this.unidadIdSeleccionada > 0) {    
      const insumoRepetido = this.insumosElegidos.find(i => i.insumoId == this.insumoIdSeleccionado)
      if(insumoRepetido)
        {
          this.noti.warning("Este insumo ya fue seleccionado en la lista")
          return;
        }
      const insumoBase = this.listaInsumos.find(i => i.id == this.insumoIdSeleccionado);    
      const unidadBase = this.listaUnidades.find(u => u.id == this.unidadIdSeleccionada);
      if (insumoBase && unidadBase) {
        this.insumosElegidos.push({
          insumoId: Number(this.insumoIdSeleccionado),
          nombreInsumo: insumoBase.nombreInsumo,
          cantidad: this.cantidadIngresada,
          unidadMedidaId: Number(this.unidadIdSeleccionada), // Guardamos el ID
          unidadMedida: unidadBase.abreviatura // Para mostrar en la tablita del modal
        });
        // Reset campos
        this.insumoIdSeleccionado = 0;
        this.cantidadIngresada = 0;
        this.unidadIdSeleccionada = 0;
      }
    } else {
      this.noti.warning('Por favor seleccione insumo, cantidad y unidad.');
    }
  }
    insumoYaEnLista(): boolean {
  return this.insumosElegidos.some(i => i.insumoId == this.insumoIdSeleccionado);
   }

  quitarInsumo(index: number) {
    this.insumosElegidos.splice(index, 1);
  }

  async crearUnidad() {
    const nombre = await this.noti.prompt({ titulo: 'Nueva unidad', texto: 'Nombre de la unidad', placeholder: 'ej: Kilogramos' });
    if (!nombre) return;
    const abreviatura = await this.noti.prompt({ titulo: 'Nueva unidad', texto: 'Abreviatura', placeholder: 'ej: Kg' });
    if (!abreviatura) return;

    this.unidadService.crear({ nombre: nombre.trim(), abreviatura: abreviatura.trim() }).subscribe({
      next: () => {
        this.noti.success('Unidad creada');
        this.cargarUnidades();
      }
    });
  }

  async eliminarUnidadDeLista() {
    if (!this.unidadIdSeleccionada) return;
    const unidad = this.listaUnidades.find(u => u.id == this.unidadIdSeleccionada);
    const ok = await this.noti.confirm({ titulo: '¿Eliminar unidad?', texto: `Se eliminará "${unidad?.nombre}".`, peligro: true });
    if (ok) {
      this.unidadService.eliminar(this.unidadIdSeleccionada).subscribe({
        next: () => {
          this.noti.success('Unidad eliminada');
          this.unidadIdSeleccionada = 0;
          this.cargarUnidades();
        }
      });
    }
  }
  get recetasPaginadas() {
  const listaAVisualizar = this.recetasFiltradas || []; 
  const inicio = (this.paginaActual - 1) * this.itemsPorPagina;
  const fin = inicio + this.itemsPorPagina;
  
  return listaAVisualizar.slice(inicio, fin);
}
  guardarReceta() {
    if (this.form.invalid) return;
    this.cargando = true;
    this.mensajeError = null;
    if (this.isEditing && this.recetaIdSeleccionada) {
      this.recetaService.update(this.recetaIdSeleccionada, this.form.value).subscribe({
        next: () => { this.closeModal(); this.loadRecetas(); },
        error: (err) => {
          this.cargando = false
           console.error("Error completo:", err); 
  let textoError = err.error?.message || err.error || 'Error inesperado';
  if (typeof textoError === 'string' && textoError.includes('\n')) {
    textoError = textoError.split('\n')[0];
  }
  this.mensajeError = textoError.replace('System.Exception: ', '').trim();
        }
      });
    } else {
      const recetaParaEnviar = {
        ...this.form.value,
        recetaInsumos: this.insumosElegidos.map(i => ({
          insumoId: i.insumoId,
          cantidad: i.cantidad,
          unidadMedidaId: i.unidadMedidaId // <--- AGREGADO PARA EL BACKEND
        }))
      };
      this.recetaService.create(recetaParaEnviar).subscribe({
        next: () => {
          this.noti.success('¡Receta creada con éxito!');
          this.closeModal();
          this.loadRecetas();
        },
        error: (err) => { this.cargando = false;
          console.error("Error completo:", err);
  let textoError = err.error?.message || err.error || 'Error inesperado';
  if (typeof textoError === 'string' && textoError.includes('\n')) {
    textoError = textoError.split('\n')[0];
  }
  this.mensajeError = textoError.replace('System.Exception: ', '').trim();
         }
      });
    }
  }

  loadRecetas(): void {
    this.cargando = true;
    this.recetaService.getAll(this.filtroNombre, this.filtroEstilo, this.filtroEstado)
      .subscribe({
        next: (data) => {
          this.recetas = data ?? [];
          this.recetasFiltradas = this.recetas;
          this.cargando = false;
        },
        error: () => this.cargando = false
      });
  }

  openCreate() {
    this.mensajeError = null;
    this.isEditing = false;
    this.form.reset({ batchSizeLitros: 20, estado: 'Activa', estilo: '' });
    this.insumosElegidos = []; 
    this.insumoIdSeleccionado = 0;
    this.unidadIdSeleccionada = 0;
    this.cantidadIngresada = 0;
    this.showModal = true;
  }

  closeModal() { this.showModal = false; }
  aplicarFiltros() { this.loadRecetas(); }
  limpiarFiltros() { this.filtroNombre = ''; this.filtroEstilo = ''; this.filtroEstado = 'Activa'; this.aplicarFiltros(); }  
  openEdit(r: any) { 
    this.mensajeError = null;
    this.isEditing = true;
    this.recetaIdSeleccionada = r.idReceta;
    this.form.patchValue({
      nombre: r.nombre, estilo: r.estilo, batchSizeLitros: r.batchSizeLitros,
      notas: r.notas, estado: r.estado
    });
    this.showModal = true;
  }

  toggleEstado(r: any) {
    const nuevoEstado = r.estado === 'Activa' ? 'Inactiva' : 'Activa';
    this.recetaService.update(r.idReceta, { ...r, estado: nuevoEstado }).subscribe({
      next: () => this.loadRecetas()
    });
  }
  async duplicarReceta(id: number) {
  const ok = await this.noti.confirm({ titulo: '¿Duplicar receta?', texto: 'Se creará una copia de esta receta.' });
  if (ok) {
    this.recetaService.duplicarReceta(id).subscribe({
      next: (nuevoId) => {
        this.router.navigate(['/recetas/detalle', nuevoId]);
      },
      error: (err) => this.noti.error('Error al duplicar la receta')
    });
  }
}
}