import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Pencil, Trash2, Plus } from 'lucide-angular'; // Agregado Lucide
import { RolService } from '../../services/rol';

@Component({
  selector: 'app-roles-gestion',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule], // Agregado LucideAngularModule
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
    if (this.formNombre.trim() === '') return;
    if (this.idEdicion) {
      const rolEditado = { id: this.idEdicion, nombre: this.formNombre, descripcion: this.formDescripcion };
      this.rolService.editarRol(this.idEdicion, rolEditado).subscribe({
        next: () => {
          alert('¡Rol editado con éxito!');
          this.obtenerRoles();
          this.cerrarYLimpiar();
        },
        error: (e) => alert('Error al editar el rol.')
      });
    } else {
      const nuevoRol = { nombre: this.formNombre, descripcion: this.formDescripcion };
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
        error: (e) => alert('No se pudo eliminar el rol (tal vez esté en uso).')
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