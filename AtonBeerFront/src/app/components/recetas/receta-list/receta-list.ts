import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import {
  LucideAngularModule,
  Search, Plus, Pencil, FileText, X, Filter, Beer, ChevronDown, Trash2
} from 'lucide-angular';
import { RecetaService, Receta } from '../../../services/receta';

@Component({
  selector: 'app-receta-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, LucideAngularModule],
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
    this.form.reset({ batchSizeLitros: 20, estado: 'Activa', version: '1.0' });
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
        this.loadRecetas(); // Recarga la lista para mostrar la nueva
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
  verDetalle(r: any) { console.log('Detalle', r); }
}