import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Pencil, Trash2, Plus } from 'lucide-angular';
import { RolService } from '../../services/rol';

@Component({
  selector: 'app-roles-gestion',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './roles-gestion.html',
  styleUrl: './roles-gestion.css',
})
export class RolesGestion implements OnInit {
  // Íconos para el HTML
  readonly Pencil = Pencil;
  readonly Trash2 = Trash2;
  readonly Plus = Plus;

  mostrarFormulario = false;
  idEdicion: number | null = null;
  formNombre = '';
  formDescripcion = '';
  roles: any[] = [];

  constructor(private rolService: RolService) {}

  ngOnInit(): void {
    this.obtenerRoles();
  }

  obtenerRoles() {
    this.rolService.getRoles().subscribe({
      next: (datos) => { this.roles = datos; },
      error: (e) => console.error('Error al cargar roles:', e),
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
    const nombreIngresado = this.formNombre.trim();
    if (nombreIngresado === '') return;

    
    const nombreDuplicado = this.roles.some(rol => 
      rol.nombre.toLowerCase() === nombreIngresado.toLowerCase() && 
      rol.id !== this.idEdicion
    );

    if (nombreDuplicado) {
      alert('Ya existe un rol con ese nombre. Por favor, ingresá uno distinto.');
      return;
    }

    if (this.idEdicion) {
      const rolEditado = { id: this.idEdicion, nombre: nombreIngresado, descripcion: this.formDescripcion };
      this.rolService.editarRol(this.idEdicion, rolEditado).subscribe({
        next: () => {
          alert('¡Rol editado con éxito!');
          this.obtenerRoles();
          this.cerrarYLimpiar();
        },
        error: (e) => alert('Error al editar el rol.')
      });
    } else {
      const nuevoRol = { nombre: nombreIngresado, descripcion: this.formDescripcion };
      this.rolService.crearRol(nuevoRol).subscribe({
        next: () => {
          alert('¡Rol creado con éxito!');
          this.obtenerRoles();
          this.cerrarYLimpiar();
        },
        error: (e) => alert('Error al crear el rol.')
      });
    }
  }

  eliminarRol(id: number) {
    if (confirm('¿Estás seguro de que querés eliminar este rol?')) {
      this.rolService.eliminarRol(id).subscribe({
        next: () => { this.obtenerRoles(); },
        error: (e) => {
          let mensaje = 'No se pudo eliminar el rol (tal vez esté en uso).';
          if (typeof e.error === 'string') mensaje = e.error;
          else if (e.error?.message) mensaje = e.error.message;
          alert(mensaje);
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