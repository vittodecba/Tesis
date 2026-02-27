import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import {
  LucideAngularModule,
  Search, Plus, Pencil, FileText, X, Filter, Beer, ChevronDown, Trash2, Save
} from 'lucide-angular';
import { RecetaService, Receta } from '../../../services/receta';
import { InsumoService } from '../../../services/insumo.service';
import { UnidadMedidaService } from '../../../services/unidadMedida'; // <--- IMPORTANTE

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
  Beer = Beer; ChevronDown = ChevronDown; Trash2 = Trash2; Save = Save;

  recetas: Receta[] = [];
  cargando = false;
  showModal = false; 
  isEditing = false; 
  recetaIdSeleccionada: number | null = null; 
  form: FormGroup;

  filtroNombre: string = '';
  filtroEstilo: string = '';
  filtroEstado: string = ''; 
  estilos: string[] = ['IPA', 'Stout', 'Golden', 'Honey'];
  
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
    private unidadService: UnidadMedidaService // <--- INYECTADO
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required]],
      estilo: ['', [Validators.required]],
      batchSizeLitros: [20, [Validators.required, Validators.min(1)]],
      notas: [''],
      estado: ['Activa'],
      version: ['1.0']
    });
  }

  ngOnInit(): void {
    this.loadRecetas();
    this.cargarInsumos();
    this.cargarUnidades(); // <--- LLAMADA INICIAL
  }

  cargarUnidades(): void {
    this.unidadService.getUnidades().subscribe({
      next: (data) => this.listaUnidades = data
    });
  }

  cargarInsumos(): void {
    this.insumoService.obtenerInsumos().subscribe({
      next: (data) => this.listaInsumos = data
    });
  }

  agregarInsumoALista() {
    if (this.insumoIdSeleccionado > 0 && this.cantidadIngresada > 0 && this.unidadIdSeleccionada > 0) {    
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
      alert('Por favor seleccione insumo, cantidad y unidad.');
    }
  }

  quitarInsumo(index: number) {
    this.insumosElegidos.splice(index, 1);
  }

  crearUnidad() {
    const nombre = prompt("Nombre de la unidad (ej: Kilogramos):");
    if (!nombre) return;
    const abreviatura = prompt("Abreviatura (ej: Kg):");
    if (!abreviatura) return;

    this.unidadService.crear({ nombre: nombre.trim(), abreviatura: abreviatura.trim() }).subscribe({
      next: () => {
        alert('Unidad creada');
        this.cargarUnidades();
      }
    });
  }

  eliminarUnidadDeLista() {
    if (!this.unidadIdSeleccionada) return;
    const unidad = this.listaUnidades.find(u => u.id == this.unidadIdSeleccionada);
    if (confirm(`¿Eliminar "${unidad?.nombre}"?`)) {
      this.unidadService.eliminar(this.unidadIdSeleccionada).subscribe({
        next: () => {
          alert('Unidad eliminada');
          this.unidadIdSeleccionada = 0;
          this.cargarUnidades();
        }
      });
    }
  }

  // --- RESTO DE TUS MÉTODOS (loadRecetas, openCreate, guardarReceta, etc.) ---
  // IMPORTANTE: En guardarReceta(), asegúrate de enviar el unidadMedidaId:
  
  guardarReceta() {
    if (this.form.invalid) return;
    this.cargando = true;

    if (this.isEditing && this.recetaIdSeleccionada) {
      this.recetaService.update(this.recetaIdSeleccionada, this.form.value).subscribe({
        next: () => { this.closeModal(); this.loadRecetas(); },
        error: () => this.cargando = false
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
          alert('¡Receta creada con éxito!');
          this.closeModal();
          this.loadRecetas(); 
        },
        error: () => { this.cargando = false; alert('Error al crear la receta.'); }
      });
    }
  }

  loadRecetas(): void {
    this.cargando = true;
    this.recetaService.getAll(this.filtroNombre, this.filtroEstilo, this.filtroEstado)
      .subscribe({
        next: (data) => {
          this.recetas = data ?? [];
          this.cargando = false;
        },
        error: () => this.cargando = false
      });
  }

  openCreate() {
    this.isEditing = false;
    this.form.reset({ batchSizeLitros: 20, estado: 'Activa', version: '1.0', estilo: '' });
    this.insumosElegidos = []; 
    this.insumoIdSeleccionado = 0;
    this.unidadIdSeleccionada = 0;
    this.cantidadIngresada = 0;
    this.showModal = true;
  }

  closeModal() { this.showModal = false; }
  aplicarFiltros() { this.loadRecetas(); }
  limpiarFiltros() { this.filtroNombre = ''; this.filtroEstilo = ''; this.filtroEstado = ''; this.aplicarFiltros(); }
  
  openEdit(r: any) { 
    this.isEditing = true;
    this.recetaIdSeleccionada = r.idReceta;
    this.form.patchValue({
      nombre: r.nombre, estilo: r.estilo, batchSizeLitros: r.batchSizeLitros,
      notas: r.notas, estado: r.estado, version: r.version
    });
    this.showModal = true;
  }

  toggleEstado(r: any) {
    const nuevoEstado = r.estado === 'Activa' ? 'Inactiva' : 'Activa';
    this.recetaService.update(r.idReceta, { ...r, estado: nuevoEstado }).subscribe({
      next: () => this.loadRecetas()
    });
  }

  agregarEstilo() {
    const nuevo = prompt('Ingrese el nombre del nuevo estilo:');
    if (nuevo?.trim()) {
      if (!this.estilos.includes(nuevo.trim())) {
        this.estilos.push(nuevo.trim());
        this.form.patchValue({ estilo: nuevo.trim() });
      }
    }
  }
}