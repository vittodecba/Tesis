import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LucideAngularModule, Pencil, Plus, Trash2, X, Beer, Save } from 'lucide-angular'; 
import { Receta, RecetaService } from '../../../services/receta'; 
import { InsumoService } from '../../../services/insumo.service'; 
import { UnidadMedidaService } from '../../../services/unidadMedida';

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
  receta: any | null = null;
  cargando: boolean = true;
  
  Pencil = Pencil; Plus = Plus; Trash2 = Trash2; X = X; Beer = Beer; Save = Save;

  showModal = false;
  form: FormGroup;
  estilos: string[] = ['IPA', 'Stout', 'Golden', 'Honey'];

  listaInsumos: any[] = [];
  listaUnidades: any[] = [];
  insumoIdSeleccionado: number = 0;
  unidadIdSeleccionada: number = 0;
  cantidadIngresada: number = 0;
  mostrarFormInsumo: boolean = false; 

  pasos: RecetaPaso[] = []; 
  showModalPaso = false;
  editandoPaso = false;
  indiceEdicionPaso = -1;
  pasoActual: RecetaPaso = { nombre: '', descripcion: '', temperatura: 0, tiempo: 0, orden: 0 };

  constructor(
    private route: ActivatedRoute,
    private recetaService: RecetaService,
    private fb: FormBuilder,
    private insumoService: InsumoService,
    private unidadService: UnidadMedidaService
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
      this.cargarUnidades();
    }
  }

  cargarReceta(id: number): void {
    this.cargando = true;
    this.recetaService.getRecetaDetalle(id).subscribe({
      next: (data: any) => {
        this.receta = data;
        this.pasos = data.pasosElaboracion || [];
        this.cargando = false;
      },
      error: (err: any) => {
        console.error('Error al traer el detalle', err);
        this.cargando = false;
      }
    });
  }

  cargarUnidades(): void {
    this.unidadService.getUnidades().subscribe({
      next: (data) => this.listaUnidades = data
    });
  }

  onInsumoChange() {
    const insumo = this.listaInsumos.find(i => 
      (i.id == this.insumoIdSeleccionado) || (i.idInsumo == this.insumoIdSeleccionado)
    );
    if (insumo) {
      this.unidadIdSeleccionada = insumo.unidadMedidaId || insumo.idUnidadMedida || 0;
    }
  }

  getUnidadSeleccionada(): string {
    if (this.insumoIdSeleccionado == 0) return '';
    const insumo = this.listaInsumos.find(i => 
      (i.id == this.insumoIdSeleccionado) || (i.idInsumo == this.insumoIdSeleccionado)
    );
    if (!insumo) return '';
    return insumo.unidad || insumo.Unidad || insumo.unidadMedida?.abreviatura || '';
  }

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
      this.recetaService.deletePaso(this.receta!.idReceta, paso.id).subscribe({
        next: () => this.cargarReceta(this.receta!.idReceta)
      });
    }
  }

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

  cargarInsumosDisponibles(): void {
    this.insumoService.obtenerInsumos().subscribe({
      next: (data) => this.listaInsumos = data
    });
  }

  agregarInsumoDinamico(): void {
    if (!this.receta || this.insumoIdSeleccionado === 0 || this.unidadIdSeleccionada === 0) return;

    const dto = {
      insumoId: Number(this.insumoIdSeleccionado),
      cantidad: this.cantidadIngresada,
      unidadMedidaId: Number(this.unidadIdSeleccionada)
    };

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

  // 🔥 NUEVO - CREAR UNIDAD
  crearUnidad() {
    const nombre = prompt("Nombre de la unidad (ej: Kilogramos):");
    if (!nombre) return;

    const abreviatura = prompt("Abreviatura (ej: Kg):");
    if (!abreviatura) return;

    const nuevaUnidad = {
      nombre: nombre.trim(),
      abreviatura: abreviatura.trim()
    };

    this.unidadService.crear(nuevaUnidad).subscribe({
      next: () => {
        alert('Unidad creada con éxito');
        this.cargarUnidades();
      }
    });
  }

  // 🔥 NUEVO - ELIMINAR UNIDAD
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

  private resetFormInsumo() {
    this.insumoIdSeleccionado = 0;
    this.cantidadIngresada = 0;
    this.unidadIdSeleccionada = 0;
    this.mostrarFormInsumo = false;
  }
}