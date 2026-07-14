import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { FormsModule } from '@angular/forms';
import { UnidadMedidaService } from '../../services/unidadMedida';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-unidad-medida',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './unidadMedidaComponent.html'
})
export class UnidadMedidaComponent implements OnInit {
  unidades: any[] = [];
  mostrarModal = false; 
  datosForm: any = { id: 0, nombre: '', abreviatura: '', activo: true };

  constructor(private unidadService: UnidadMedidaService, private noti: NotificationService) {}

  ngOnInit(): void {
    this.listar();
  }

  listar(): void {
    this.unidadService.getUnidades().subscribe((res: any[]) => {
      this.unidades = res;
    });
  }

  abrirModal(unidad?: any): void {
    if (unidad) {
      this.datosForm = { ...unidad };
    } else {
      this.datosForm = { id: 0, nombre: '', abreviatura: '', activo: true };
    }
    this.mostrarModal = true;
  }

  guardar(): void {
    // Validación de duplicados
    const existe = this.unidades.find(u => 
      u.nombre.toLowerCase() === this.datosForm.nombre.toLowerCase() && u.id !== this.datosForm.id
    );

    if (existe) {
      this.noti.warning('Esta unidad de medida ya existe.');
      return;
    }

    if (this.datosForm.id > 0) {
      this.unidadService.actualizar(this.datosForm.id, this.datosForm).subscribe(() => {
        this.cerrarModal();
      });
    } else {
      this.unidadService.crear(this.datosForm).subscribe(() => {
        this.cerrarModal();
      });
    }
  }

  async eliminar(id: number): Promise<void> {
    const ok = await this.noti.confirm({ titulo: '¿Eliminar unidad?', peligro: true });
    if (ok) {
      this.unidadService.eliminar(id).subscribe(() => {
        this.noti.success('Unidad eliminada');
        this.listar();
      });
    }
  }

  cerrarModal(): void {
    this.mostrarModal = false;
    this.listar();
  }
}