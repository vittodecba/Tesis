import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RolService } from '../../services/rol'; 

@Component({
  selector: 'app-roles-gestion',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './roles-gestion.html',
  styleUrl: './roles-gestion.css'
})
export class RolesGestion implements OnInit {
  mostrarFormulario = false;
  
  // Variables del formulario
  idEdicion: number | null = null;
  formNombre = '';
  formDescripcion = '';

  roles: any[] = [];

  constructor(private rolService: RolService) {}

  ngOnInit(): void {
    this.obtenerRoles();
  }

  // 1. CARGAR (READ)
  obtenerRoles() {
    this.rolService.getRoles().subscribe({
      next: (datos) => {
        this.roles = datos;
      },
      error: (e) => console.error('Error al cargar roles:', e)
    });
  }

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
      // 2. EDITAR (UPDATE)
      const rolEditado = { 
        id: this.idEdicion, 
        nombre: this.formNombre, 
        descripcion: this.formDescripcion 
      };

      this.rolService.editarRol(this.idEdicion, rolEditado).subscribe({
        next: () => {
          alert('¡Rol editado con éxito!');
          this.obtenerRoles(); // Recargamos la lista real
          this.cerrarYLimpiar();
        },
        error: (e) => {
          console.error('Error al editar:', e);
          alert('Hubo un error al editar el rol.');
        }
      });

    } else {
      // 3. CREAR (CREATE)
      const nuevoRol = { 
        nombre: this.formNombre, 
        descripcion: this.formDescripcion 
      };

      this.rolService.crearRol(nuevoRol).subscribe({
        next: () => {
          alert('¡Rol creado con éxito!');
          this.obtenerRoles(); // Recargamos la lista real
          this.cerrarYLimpiar();
        },
        error: (e) => {
          console.error('Error al crear:', e);
          alert('Hubo un error al crear el rol.');
        }
      });
    }
  }

  // 4. ELIMINAR (DELETE)
  eliminarRol(id: number) {
    if (confirm('¿Estás seguro de que querés eliminar este rol?')) {
      this.rolService.eliminarRol(id).subscribe({
        next: () => {
          this.obtenerRoles(); // Recargamos la lista real
        },
        error: (e) => {
          console.error('Error al eliminar:', e);
          alert('No se pudo eliminar el rol (tal vez esté en uso).');
        }
      });
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