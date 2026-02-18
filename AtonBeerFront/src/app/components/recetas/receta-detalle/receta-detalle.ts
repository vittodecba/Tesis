import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LucideAngularModule, Pencil, Plus, Trash2, X } from 'lucide-angular'; 
import { RecetaService } from '../../../services/receta'; 

@Component({
  selector: 'app-receta-detalle',
  imports: [CommonModule, RouterModule, LucideAngularModule, FormsModule, ReactiveFormsModule], 
  templateUrl: './receta-detalle.html',
  styleUrl: './receta-detalle.css',
})
export class RecetaDetalle implements OnInit {
  receta: any = null;
  cargando: boolean = true;
  
  // Íconos
  Pencil = Pencil; Plus = Plus; Trash2 = Trash2; X = X;

  // Variables para el modal
  showModal = false;
  form: FormGroup;
  estilos: string[] = ['IPA', 'Stout', 'Golden', 'Honey'];

  constructor(
    private route: ActivatedRoute,
    private recetaService: RecetaService,
    private fb: FormBuilder
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
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = Number(idParam);
      this.cargarReceta(id);
    }
  }

  cargarReceta(id: number): void {
    this.cargando = true;
    this.recetaService.getRecetaDetalle(id).subscribe({
      next: (data: any) => {
        this.receta = data;
        this.cargando = false;
      },
      error: (err: any) => {
        console.error('Error al traer el detalle de la receta', err);
        this.cargando = false;
      }
    });
  }

  // --- LÓGICA DEL MODAL DE EDICIÓN ---
  openEdit() {
    if (this.receta.estilo && !this.estilos.includes(this.receta.estilo)) {
      this.estilos.push(this.receta.estilo);
    }

    this.form.patchValue({
      nombre: this.receta.nombre,
      estilo: this.receta.estilo,
      batchSizeLitros: this.receta.batchSizeLitros,
      notas: this.receta.notas,
      estado: this.receta.estado,
      version: this.receta.version
    });
    
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  guardarReceta() {
    if (this.form.invalid) return;

    this.recetaService.update(this.receta.idReceta, this.form.value).subscribe({
      next: () => {
        alert('¡Receta actualizada con éxito!');
        this.closeModal();
        this.cargarReceta(this.receta.idReceta); // Recarga los datos en la pantalla
      },
      error: (err: any) => {
        alert('Error al actualizar la receta.');
        console.error(err);
      }
    });
  }

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
    }
  }
}