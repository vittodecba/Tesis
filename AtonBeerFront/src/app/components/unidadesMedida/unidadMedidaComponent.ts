import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { FormsModule } from '@angular/forms';
import { UnidadMedidaService } from '../../services/unidadMedida';

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

  constructor(private unidadService: UnidadMedidaService) {}

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
      alert('Esta unidad de medida ya existe.');
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

  eliminar(id: number): void {
    if (confirm('¿Desea eliminar esta unidad?')) {
      this.unidadService.eliminar(id).subscribe(() => {
        this.listar();
      });
    }
  }

  cerrarModal(): void {
    this.mostrarModal = false;
    this.listar();
  }
}