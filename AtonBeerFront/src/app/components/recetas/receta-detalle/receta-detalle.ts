import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LucideAngularModule, Pencil, Plus, Trash2, X, Beer, Save } from 'lucide-angular'; 
import { Receta, RecetaService } from '../../../services/receta'; 
import { InsumoService } from '../../../services/insumo.service'; 

// Interfaz para el proceso de elaboración (PBI 92)
export interface RecetaPaso {
  id?: number;
  nombre: string;
  temperatura: number;
  descripcion : string;
  tiempo: number;
  orden: number;
}

@Component({
  selector: 'app-receta-detalle',
  standalone: true,
  imports: [CommonModule, RouterModule, LucideAngularModule, FormsModule, ReactiveFormsModule], 
  templateUrl: './receta-detalle.html',
  styleUrl: './receta-detalle.css',
})
export class RecetaDetalle implements OnInit {
  receta: Receta | null = null;
  cargando: boolean = true;
  
  // Iconos
  Pencil = Pencil; Plus = Plus; Trash2 = Trash2; X = X; Beer = Beer; Save = Save;

  // --- VARIABLES EDICIÓN GENERAL ---
  showModal = false;
  form: FormGroup;
  estilos: string[] = ['IPA', 'Stout', 'Golden', 'Honey'];

  // --- VARIABLES INSUMOS ---
  listaInsumos: any[] = [];
  insumoIdSeleccionado: number = 0;
  cantidadIngresada: number = 0;
  mostrarFormInsumo: boolean = false; 

  // --- VARIABLES PASOS DE COCCIÓN (PBI 92) ---
  pasos: RecetaPaso[] = []; 
  showModalPaso = false;
  editandoPaso = false;
  indiceEdicionPaso = -1;
  pasoActual: RecetaPaso = { nombre: '', descripcion: '', temperatura: 0, tiempo: 0, orden: 0 };

  constructor(
    private route: ActivatedRoute,
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
        // Asignamos los pasos que vienen del Backend al array local
        this.pasos = data.pasosElaboracion || [];
        this.cargando = false;
      },
      error: (err: any) => {
        console.error('Error al traer el detalle', err);
        this.cargando = false;
      }
    });
  }

  // LÓGICA PASOS (PBI 92 - Vinculada al Backend)
  abrirModalPaso() {
    this.editandoPaso = false;
    this.pasoActual = { nombre: '', descripcion: '', temperatura: 65, tiempo: 60, orden: this.pasos.length + 1 };
    this.showModalPaso = true;
  }

  cerrarModalPaso() {
    this.showModalPaso = false;
    this.indiceEdicionPaso = -1;
  }

  guardarPaso() {
    if (!this.receta) return;

    if (this.editandoPaso) {
      // PERSISTENCIA REAL: EDITAR (PUT)
      const idPaso = this.pasos[this.indiceEdicionPaso].id;
      if (idPaso) {
        this.recetaService.updatePaso(this.receta.idReceta, idPaso, this.pasoActual).subscribe({
          next: () => {
            this.cargarReceta(this.receta!.idReceta);
            this.cerrarModalPaso();
          }
        });
      }
    } else {
      // PERSISTENCIA REAL: CREAR (POST)
      this.recetaService.addPaso(this.receta.idReceta, this.pasoActual).subscribe({
        next: () => {
          this.cargarReceta(this.receta!.idReceta);
          this.cerrarModalPaso();
        }
      });
    }
  }

  prepararEdicion(paso: RecetaPaso, index: number) {
    this.editandoPaso = true;
    this.indiceEdicionPaso = index;
    this.pasoActual = { ...paso };
    this.showModalPaso = true;
  }

  eliminarPaso(index: number) {
    const paso = this.pasos[index];
    if (paso.id && confirm('¿Desea eliminar este paso permanentemente?')) {
      // PERSISTENCIA REAL: ELIMINAR (DELETE)
      this.recetaService.deletePaso(this.receta!.idReceta, paso.id).subscribe({
        next: () => {
          this.cargarReceta(this.receta!.idReceta);
        }
      });
    }
  }

  // LÓGICA EDICIÓN GENERAL Y ESTILOS
  openEdit() {
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

  closeModal() { this.showModal = false; }

  guardarReceta() {
    if (this.form.invalid || !this.receta) return;
    this.recetaService.update(this.receta.idReceta, this.form.value).subscribe({
      next: () => {
        alert('Receta actualizada');
        this.closeModal();
        this.cargarReceta(this.receta!.idReceta);
      }
    });
  }

  agregarEstilo() {
    const nuevo = prompt('Nuevo estilo:');
    if (nuevo && !this.estilos.includes(nuevo)) {
      this.estilos.push(nuevo.trim());
      this.form.patchValue({ estilo: nuevo.trim() });
    }
  }

  eliminarEstilo() {
    const sel = this.form.get('estilo')?.value;
    if (sel && confirm(`¿Eliminar estilo ${sel}?`)) {
      this.estilos = this.estilos.filter(e => e !== sel);
      this.form.patchValue({ estilo: '' }); 
    }
  }

  // LÓGICA INSUMOS
  cargarInsumosDisponibles(): void {
    this.insumoService.obtenerInsumos().subscribe({
      next: (data) => this.listaInsumos = data
    });
  }

  agregarInsumoDinamico(): void {
    if (!this.receta || this.insumoIdSeleccionado === 0) return;
    const dto = { insumoId: Number(this.insumoIdSeleccionado), cantidad: this.cantidadIngresada };
    this.recetaService.addInsumo(this.receta.idReceta, dto).subscribe({
      next: () => {
        this.cargarReceta(this.receta!.idReceta);
        this.resetFormInsumo();
      }
    });
  }

  eliminarInsumo(insumoId: number): void {
    if (confirm('¿Quitar ingrediente?')) {
      this.recetaService.removeInsumo(this.receta!.idReceta, insumoId).subscribe({
        next: () => this.cargarReceta(this.receta!.idReceta)
      });
    }
  }

  private resetFormInsumo() {
    this.insumoIdSeleccionado = 0;
    this.cantidadIngresada = 0;
    this.mostrarFormInsumo = false;
  }
}