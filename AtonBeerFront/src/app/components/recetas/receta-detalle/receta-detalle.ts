import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
// Fusionamos las importaciones de formularios y Lucide
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LucideAngularModule, Pencil, Plus, Trash2, X, Beer, Save } from 'lucide-angular'; 
import { Receta, RecetaService } from '../../../services/receta'; 
import { InsumoService } from '../../../services/insumo.service'; 

@Component({
  selector: 'app-receta-detalle',
  standalone: true,
  imports: [CommonModule, RouterModule, LucideAngularModule, FormsModule, ReactiveFormsModule], 
  templateUrl: './receta-detalle.html',
  styleUrl: './receta-detalle.css',
})
export class RecetaDetalle implements OnInit {
  // Usamos la interfaz Receta para mayor seguridad
  receta: Receta | null = null;
  cargando: boolean = true;
  
  // Íconos (todos los que usamos ambos)
  Pencil = Pencil; Plus = Plus; Trash2 = Trash2; X = X; Beer = Beer; Save = Save;

  // Variables para el modal de edición (de tus amigos)
  showModal = false;
  form: FormGroup;
  estilos: string[] = ['IPA', 'Stout', 'Golden', 'Honey'];

  // Variables para la gestión de insumos (tuyas)
  listaInsumos: any[] = [];
  insumoIdSeleccionado: number = 0;
  cantidadIngresada: number = 0;
  mostrarFormInsumo: boolean = false; 

  constructor(
    private route: ActivatedRoute,
    private recetaService: RecetaService,
    private fb: FormBuilder,
    private insumoService: InsumoService 
  ) {
    // Inicialización del formulario reactivo
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
      this.cargarInsumosDisponibles();
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
        console.error('Error al traer el detalle', err);
        this.cargando = false;
      }
    });
  }

  // --- MÉTODOS DE TUS AMIGOS (MODAL DE EDICIÓN) ---
  openEdit() {
    if (this.receta && this.receta.estilo && !this.estilos.includes(this.receta.estilo)) {
      this.estilos.push(this.receta.estilo);
    }

    if (this.receta) {
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
  }

  closeModal() {
    this.showModal = false;
  }

  guardarReceta() {
    if (this.form.invalid || !this.receta) return;

    this.recetaService.update(this.receta.idReceta, this.form.value).subscribe({
      next: () => {
        alert('¡Receta actualizada con éxito!');
        this.closeModal();
        this.cargarReceta(this.receta!.idReceta);
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

  // --- TUS MÉTODOS (GESTIÓN DE INSUMOS) ---
  cargarInsumosDisponibles(): void {
    this.insumoService.obtenerInsumos().subscribe({
      next: (data) => this.listaInsumos = data,
      error: (err) => console.error('Error al cargar insumos', err)
    });
  }

  agregarInsumoDinamico(): void {
    if (!this.receta || this.insumoIdSeleccionado === 0 || this.cantidadIngresada <= 0) {
      alert('Datos inválidos');
      return;
    }

    const dto = {
      insumoId: Number(this.insumoIdSeleccionado),
      cantidad: this.cantidadIngresada
    };

    this.recetaService.addInsumo(this.receta.idReceta, dto).subscribe({
      next: () => {
        this.cargarReceta(this.receta!.idReceta);
        this.resetFormInsumo();
      },
      error: (err) => alert('Error al agregar el insumo')
    });
  }

  eliminarInsumo(insumoId: number): void {
    if (confirm('¿Estás seguro de quitar este ingrediente?')) {
      this.recetaService.removeInsumo(this.receta!.idReceta, insumoId).subscribe({
        next: () => {
          this.cargarReceta(this.receta!.idReceta);
        },
        error: (err) => alert('No se pudo eliminar el insumo')
      });
    }
  }

  private resetFormInsumo() {
    this.insumoIdSeleccionado = 0;
    this.cantidadIngresada = 0;
    this.mostrarFormInsumo = false;
  }
}