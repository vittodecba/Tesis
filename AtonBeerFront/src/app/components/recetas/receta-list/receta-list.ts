import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import {
  LucideAngularModule,
  Search, Plus, Pencil, FileText, X, Filter, Beer, ChevronDown, Trash2
} from 'lucide-angular';
import { RecetaService, Receta } from '../../../services/receta';

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
  Beer = Beer; ChevronDown = ChevronDown; Trash2 = Trash2;

  recetas: Receta[] = [];
  cargando = false;
  showModal = false; 
  form: FormGroup;

  filtroNombre: string = '';
  filtroEstilo: string = '';
  filtroEstado: string = ''; 

  // --- NUEVA LISTA DINÁMICA DE ESTILOS ---
  estilos: string[] = ['IPA', 'Stout', 'Golden', 'Honey'];

  constructor(private recetaService: RecetaService, private fb: FormBuilder) {
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
    this.form.reset({ batchSizeLitros: 20, estado: 'Activa', version: '1.0', estilo: '' });
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  guardarReceta() {
    if (this.form.invalid) return;

    this.cargando = true;
    this.recetaService.create(this.form.value).subscribe({
      next: () => {
        alert('¡Receta creada con éxito!');
        this.closeModal();
        this.loadRecetas(); 
      },
      error: (err) => {
        this.cargando = false;
        alert('Error al crear la receta.');
        console.error(err);
      }
    });
  }

  aplicarFiltros() { this.loadRecetas(); }
  
  limpiarFiltros() {
    this.filtroNombre = ''; 
    this.filtroEstilo = ''; 
    this.filtroEstado = '';
    this.aplicarFiltros();
  }

  openEdit(r: any) { console.log('Editar', r); }
  
  toggleEstado(r: any) { console.log('Cambiar estado de', r.nombre); } 

  // --- NUEVAS FUNCIONES PARA LOS ESTILOS ---
  agregarEstilo() {
    const nuevoEstilo = prompt('Ingrese el nombre del nuevo estilo de cerveza:');
    if (nuevoEstilo && nuevoEstilo.trim() !== '') {
      const estiloLimpio = nuevoEstilo.trim();
      if (!this.estilos.includes(estiloLimpio)) {
        this.estilos.push(estiloLimpio);
        this.form.patchValue({ estilo: estiloLimpio }); // Lo selecciona automáticamente
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
      this.form.patchValue({ estilo: '' }); // Limpia la selección
      
      // Si el estilo eliminado estaba en el filtro, limpiamos el filtro también
      if (this.filtroEstilo === estiloSeleccionado) {
        this.filtroEstilo = '';
        this.aplicarFiltros();
      }
    }
  }
}