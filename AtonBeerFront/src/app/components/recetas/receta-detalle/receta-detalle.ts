import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LucideAngularModule, Pencil, Plus, Trash2, X, Beer, Save, Check} from 'lucide-angular'; 
import { Receta, RecetaService} from '../../../services/receta';
import { InsumoService } from '../../../services/insumo.service';
import { UnidadMedidaService } from '../../../services/unidadMedida';
import { NotificationService } from '../../../core/services/notification.service';

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
  
  Pencil = Pencil; Plus = Plus; Trash2 = Trash2; X = X; Beer = Beer; Save = Save; Check = Check;

  showModal = false;
  form: FormGroup;
  estilos: string[] = [];
  estilosDefault: string[] = ['IPA', 'Stout', 'Golden', 'Honey'];

  listaInsumos: any[] = [];
  listaUnidades: any[] = [];
  InsumoDuplicado: boolean = false
  mostrarFormInsumo: boolean = false; 
  editandoInsumo = false;
  insumoIdSeleccionado: number = 0;
  cantidadIngresada: number = 0;
  unidadIdSeleccionada: number = 0;
  mensajeErrorNombre: string = '';

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
    private unidadService: UnidadMedidaService,
    private noti: NotificationService,
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required]],
      estilo: ['', [Validators.required]],
      batchSizeLitros: [20, [Validators.required, Validators.min(1)]],
      notas: [''],
      estado: ['Activa'],
    });
  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = Number(idParam); 
      this.cargarEstilosLocales()     
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
//Pasos Elaboracion//
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

  async eliminarPaso(index: number) {
    const paso = this.pasos[index];
    if (!paso.id) return;
    const ok = await this.noti.confirm({ titulo: '¿Eliminar paso?', texto: 'Se eliminará permanentemente.', peligro: true });
    if (ok) {
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
      });
      this.showModal = true;
    }
  }

  closeModal() { this.showModal = false; }

  guardarReceta() {
    if (this.form.invalid || !this.receta) return;
    this.recetaService.update(this.receta.idReceta, this.form.value).subscribe({
      next: () => {
        this.noti.success('Receta actualizada');
        this.closeModal();
        this.cargarReceta(this.receta!.idReceta);
      },
      error: (err) => {      
      this.mensajeErrorNombre = err.error?.message || err.error || "El nombre ya está en uso.";
    }
    });
  }
///INSUMOS
limpiarFormInsumo() {
    this.insumoIdSeleccionado = 0;
    this.cantidadIngresada = 0;
    this.unidadIdSeleccionada = 0;
    this.editandoInsumo = false;
    this.mostrarFormInsumo = false;
    this.InsumoDuplicado = false;
  }

  cargarInsumosDisponibles(): void {
    this.insumoService.obtenerInsumos().subscribe({
      next: (data) => this.listaInsumos = data
    });
  } 

  verificarDuplicado() {
  if (!this.receta || !this.receta.recetaInsumos) {
    this.InsumoDuplicado = false;
    return;
  }
  this.InsumoDuplicado = this.receta.recetaInsumos.some(
    (item: any) => Number(item.insumoId) == Number(this.insumoIdSeleccionado)
  );
  console.log("¿Está duplicado?", this.InsumoDuplicado);
}
  prepararEdicionInsumo(item: any) {
  this.mostrarFormInsumo = true;
  this.editandoInsumo = true; // Cambiamos el modo
  
  // Cargamos los datos en el formulario
  this.insumoIdSeleccionado = item.insumoId;
  this.cantidadIngresada = item.cantidad;
  this.unidadIdSeleccionada = item.unidadMedidaId;
}

  agregarInsumoDinamico(suma: boolean): void {
    if (!this.receta || this.insumoIdSeleccionado === 0 || this.unidadIdSeleccionada === 0) return;

    const dto = {
      insumoId: Number(this.insumoIdSeleccionado),
      cantidad: this.cantidadIngresada,
      unidadMedidaId: Number(this.unidadIdSeleccionada)
    };
    if (this.editandoInsumo) {
    this.recetaService.actualizarInsumo(this.receta.idReceta, dto, suma).subscribe({
      next: () => {
        this.cargarReceta(this.receta.idReceta);
        this.limpiarFormInsumo();
        this.editandoInsumo = false;
      },
      error: (err) => this.noti.error(err.error || "No se pudo actualizar el insumo.")
    });
  }else{

    this.recetaService.addInsumo(this.receta.idReceta, dto).subscribe({
      next: () => {
        this.cargarReceta(this.receta!.idReceta);
        this.limpiarFormInsumo();
      },
      error: (err) => this.noti.error(err.error || "El insumo ya existe en la receta.")
    });
  }
}
prepararNuevoInsumo() {
  if (this.mostrarFormInsumo) {
    // Si el formulario ya está abierto, lo cerramos y limpiamos
    this.limpiarFormInsumo();
  } else {
    // Si está cerrado, nos aseguramos de que no entre en modo edición
    this.limpiarFormInsumo();
    this.mostrarFormInsumo = true;
  }
}

  async eliminarInsumo(insumoId: number): Promise<void> {
    const ok = await this.noti.confirm({ titulo: '¿Quitar ingrediente?', peligro: true });
    if (ok) {
      this.recetaService.removeInsumo(this.receta!.idReceta, insumoId).subscribe({
        next: () => this.cargarReceta(this.receta!.idReceta)
      });
    }
  }

  //CREAR UNIDAD
  async crearUnidad() {
    const nombre = await this.noti.prompt({ titulo: 'Nueva unidad', texto: 'Nombre de la unidad', placeholder: 'ej: Kilogramos' });
    if (!nombre) return;

    const abreviatura = await this.noti.prompt({ titulo: 'Nueva unidad', texto: 'Abreviatura', placeholder: 'ej: Kg' });
    if (!abreviatura) return;

    const nuevaUnidad = {
      nombre: nombre.trim(),
      abreviatura: abreviatura.trim()
    };

    this.unidadService.crear(nuevaUnidad).subscribe({
      next: () => {
        this.noti.success('Unidad creada con éxito');
        this.cargarUnidades();
      }
    });
  }

  //ELIMINAR UNIDAD
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
}