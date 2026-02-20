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

@Component({
  selector: 'app-receta-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, LucideAngularModule, RouterModule],
  templateUrl: './receta-list.html',
  styleUrls: ['./receta-list.css']
})
export class RecetaListComponent implements OnInit {
  // Iconos
  Search = Search; Plus = Plus; Pencil = Pencil; 
  FileText = FileText; X = X; Filter = Filter; 
  Beer = Beer; ChevronDown = ChevronDown; Trash2 = Trash2; Save = Save;

  recetas: Receta[] = [];
  cargando = false;
  showModal = false; 
  form: FormGroup;

  filtroNombre: string = '';
  filtroEstilo: string = '';
  filtroEstado: string = ''; 

  estilos: string[] = ['IPA', 'Stout', 'Golden', 'Honey'];

  // --- VARIABLES FUSIONADAS (EDICIÓN + INSUMOS) ---
  isEditing = false;
  idEditando: number | null = null;
  listaInsumos: any[] = []; 
  insumosElegidos: any[] = []; 
  insumoIdSeleccionado: number = 0;
  cantidadIngresada: number = 0;

  constructor(
    private recetaService: RecetaService, 
    private fb: FormBuilder,
    private insumoService: InsumoService
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
  }

  cargarInsumos(): void {
    this.insumoService.obtenerInsumos().subscribe({
      next: (data) => {
        this.listaInsumos = data;
      },
      error: (err) => console.error('Error al cargar insumos', err)
    });
  }

  agregarInsumoALista() {
    if (this.insumoIdSeleccionado > 0 && this.cantidadIngresada > 0) {    
      const insumoBase = this.listaInsumos.find(i => i.id == this.insumoIdSeleccionado);    
      if (insumoBase) {
        this.insumosElegidos.push({
          insumoId: Number(this.insumoIdSeleccionado),
          nombreInsumo: insumoBase.nombreInsumo,
          cantidad: this.cantidadIngresada,
          unidadMedida: insumoBase.unidadMedida
        });
        this.insumoIdSeleccionado = 0;
        this.cantidadIngresada = 0;
      }
    } else {
      alert('Por favor seleccione un insumo y una cantidad válida.');
    }
  }

  quitarInsumo(index: number) {
    this.insumosElegidos.splice(index, 1);
  }

  loadRecetas(): void {
    this.cargando = true;
    this.recetaService.getAll(this.filtroNombre, this.filtroEstilo, this.filtroEstado)
      .subscribe({
        next: (data) => {
          this.recetas = data ?? [];
          const estilosBD = this.recetas.map(r => r.estilo).filter(e => e);
          estilosBD.forEach(e => {
            if (!this.estilos.includes(e)) this.estilos.push(e);
          });
          this.cargando = false;
        },
        error: () => this.cargando = false
      });
  }

  openCreate() {
    this.isEditing = false;
    this.idEditando = null;
    this.form.reset({ batchSizeLitros: 20, estado: 'Activa', version: '1.0', estilo: '' });
    this.insumosElegidos = []; 
    this.insumoIdSeleccionado = 0;
    this.cantidadIngresada = 0;
    this.showModal = true;
  }

  openEdit(r: any) {
    this.isEditing = true;
    this.idEditando = r.idReceta;
    if (r.estilo && !this.estilos.includes(r.estilo)) {
      this.estilos.push(r.estilo);
    }
    this.form.patchValue({
      nombre: r.nombre,
      estilo: r.estilo,
      batchSizeLitros: r.batchSizeLitros,
      notas: r.notas,
      estado: r.estado,
      version: r.version
    });
    this.insumosElegidos = r.recetaInsumos || [];
    this.showModal = true; 
  }

  closeModal() {
    this.showModal = false;
    this.isEditing = false;
    this.idEditando = null;
  }

  guardarReceta() {
    if (this.form.invalid) return;

    // Combinamos los datos del formulario con el array de insumos
    const recetaParaEnviar = {
      ...this.form.value,
      recetaInsumos: this.insumosElegidos.map(i => ({
        insumoId: i.insumoId,
        cantidad: i.cantidad
      }))
    };

    this.cargando = true;

    if (this.isEditing && this.idEditando) {
      // --- LÓGICA DE ACTUALIZACIÓN (HEAD) ---
      this.recetaService.update(this.idEditando, recetaParaEnviar).subscribe({
        next: () => {
          alert('¡Receta actualizada con éxito!');
          this.closeModal();
          this.loadRecetas(); 
        },
        error: (err: any) => {
          this.cargando = false;
          alert('Error al actualizar la receta.');
          console.error(err);
        }
      });
    } else {
      // --- LÓGICA DE CREACIÓN (TU PBI) ---
      this.recetaService.create(recetaParaEnviar).subscribe({
        next: () => {
          alert('¡Receta creada con éxito!');
          this.closeModal();
          this.loadRecetas(); 
        },
        error: (err: any) => {
          this.cargando = false;
          alert('Error al crear la receta.');
          console.error(err);
        }
      });
    }
  }

  aplicarFiltros() { this.loadRecetas(); }
  
  limpiarFiltros() {
    this.filtroNombre = ''; 
    this.filtroEstilo = ''; 
    this.filtroEstado = '';
    this.aplicarFiltros();
  }
  
  toggleEstado(r: any) { console.log('Cambiar estado de', r.nombre); } 

  agregarEstilo() {
    const nuevoEstilo = prompt('Ingrese el nombre del nuevo estilo de cerveza:');
    if (nuevoEstilo && nuevoEstilo.trim() !== '') {
      const estiloLimpio = nuevoEstilo.trim();
      if (!this.estilos.includes(estiloLimpio)) {
        this.estilos.push(estiloLimpio);
        this.form.patchValue({ estilo: estiloLimpio }); 
      } else {
        alert('Ese estilo ya existe en la lista.');
      }
    }
  }

  eliminarEstilo() {
    const estiloSeleccionado = this.form.get('estilo')?.value;
    if (!estiloSeleccionado) {
      alert('Primero seleccione un estilo de la lista para eliminarlo.');
      return;
    }
    const confirmacion = confirm(`¿Está seguro que desea eliminar el estilo "${estiloSeleccionado}"?`);
    if (confirmacion) {
      this.estilos = this.estilos.filter(e => e !== estiloSeleccionado);
      this.form.patchValue({ estilo: '' }); 
      if (this.filtroEstilo === estiloSeleccionado) {
        this.filtroEstilo = '';
        this.aplicarFiltros();
      }
    }
  }
}