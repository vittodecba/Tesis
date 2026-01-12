import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-roles-gestion',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './roles-gestion.html',
  styleUrl: './roles-gestion.css'
})
export class RolesGestion {
  mostrarFormulario = false;
  
  // Variables del formulario
  idEdicion: number | null = null;
  formNombre = '';
  formDescripcion = '';

  // Datos simulados con descripción
  roles = [
    { id: 1, nombre: 'Cocinero', descripcion: 'Utiliza el sistema para registrar y consultar procesos productivos. Carga datos de recetas, fermentaciones, uso de insumos y estado de barriles. Su foco está en la operación diaria de producción.' },
    { id: 2, nombre: 'Responsable de planta', descripcion: 'Supervisa la producción. Controla stock de insumos, barriles y latas, valida registros hechos por los cocineros y monitorea el estado general del proceso productivo.' },
    { id: 3, nombre: 'Responsable de pedidos', descripcion: 'Gestiona el módulo comercial operativo. Registra pedidos, controla entregas, actualiza estados de pedidos y verifica disponibilidad de stock para la venta.' },
    { id: 4, nombre: 'Gerente', descripcion: 'Gestiona clientes y realiza seguimiento de pedidos, controlando su estado y la información asociada.' },
    { id: 5, nombre: 'Gerente mayor', descripcion: 'Consulta ventas y reportes de ventas, utilizando la información para análisis y toma de decisiones.' }
  ];

  toggleFormulario() {
    this.mostrarFormulario = !this.mostrarFormulario;
    this.limpiarForm();
  }

  cargarDatosEdicion(rol: any) {
    this.mostrarFormulario = true;
    this.idEdicion = rol.id;
    this.formNombre = rol.nombre;
    this.formDescripcion = rol.descripcion;
  }

  guardarRol() {
    if (this.formNombre.trim() === '') return;

    if (this.idEdicion) {
      // Editar
      const index = this.roles.findIndex(r => r.id === this.idEdicion);
      if (index !== -1) {
        this.roles[index].nombre = this.formNombre;
        this.roles[index].descripcion = this.formDescripcion;
      }
    } else {
      // Crear
      const nuevoId = this.roles.length > 0 ? Math.max(...this.roles.map(r => r.id)) + 1 : 1;
      this.roles.push({ 
        id: nuevoId, 
        nombre: this.formNombre, 
        descripcion: this.formDescripcion 
      });
    }
    this.cerrarYLimpiar();
  }

  eliminarRol(id: number) {
    if (confirm('¿Estás seguro de que querés eliminar este rol?')) {
      this.roles = this.roles.filter(r => r.id !== id);
    }
  }

  limpiarForm() {
    this.idEdicion = null;
    this.formNombre = '';
    this.formDescripcion = '';
  }

  cerrarYLimpiar() {
    this.limpiarForm();
    this.mostrarFormulario = false;
  }
}