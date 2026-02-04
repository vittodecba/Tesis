import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UsuarioService } from '../../services/usuario.service';
import { RolService } from '../../services/rol';
import { Usuario, UsuarioCreate, UsuarioUpdate } from '../../Interfaces/usuario.interface';

@Component({
  selector: 'app-usuarios',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './usuarios.html',
  styleUrls: ['./usuarios.css'],
})
export class UsuariosComponent implements OnInit {
  usuarios: Usuario[] = [];
  roles: any[] = [];
  mostrarModal: boolean = false;
  esEdicion: boolean = false;
  tituloModal: string = 'Nuevo Usuario';
  verInactivos: boolean = false;

  datosForm = {
    id: 0,
    nombre: '',
    apellido: '',
    email: '',
    password: '',
    confirmarPassword: '',
    rolId: 0,
  };

  constructor(
    private usuarioService: UsuarioService,
    private rolService: RolService,
  ) {}

  ngOnInit(): void {
    this.cargarUsuarios();
    this.cargarRoles();
  }

  cargarUsuarios() {
    this.usuarioService.getUsuarios(this.verInactivos).subscribe({
      next: (data) => (this.usuarios = data),
      error: (e) => console.error(e),
    });
  }

  cargarRoles() {
    this.rolService.getRoles().subscribe({
      next: (data) => (this.roles = data),
    });
  }

  abrirModal(usuario?: Usuario) {
    this.mostrarModal = true;
    if (usuario) {
      this.esEdicion = true;
      this.tituloModal = 'Editar Usuario';
      this.datosForm = {
        id: usuario.id,
        nombre: usuario.nombre,
        apellido: usuario.apellido,
        email: usuario.email,
        password: '',
        confirmarPassword: '',
        rolId: usuario.rolId || 0,
      };
    } else {
      this.esEdicion = false;
      this.tituloModal = 'Nuevo Usuario';
      this.limpiarFormulario();
    }
  }

  cerrarModal() {
    this.mostrarModal = false;
    this.limpiarFormulario();
  }

  guardar() {
    if (this.esEdicion) {
      const dto: UsuarioUpdate = {
        id: this.datosForm.id,
        nombre: this.datosForm.nombre,
        apellido: this.datosForm.apellido,
        email: this.datosForm.email,
        rolId: Number(this.datosForm.rolId),
        activo: true, // O mantener el actual
      };

      this.usuarioService.updateUsuario(this.datosForm.id, dto).subscribe(() => {
        alert('¡Usuario modificado!');
        this.cerrarModal();
        this.cargarUsuarios();
      });
    } else {
      // Lógica de creación (la que ya tenías)
      if (this.datosForm.password !== this.datosForm.confirmarPassword) {
        alert('Las contraseñas no coinciden');
        return;
      }
      const dto: UsuarioCreate = { ...this.datosForm, rolId: Number(this.datosForm.rolId) };
      this.usuarioService.createUsuario(dto).subscribe(() => {
        alert('¡Usuario creado!');
        this.cerrarModal();
        this.cargarUsuarios();
      });
    }
  }

  toggleActivo(usuario: Usuario) {
    const accion = usuario.activo ? 'desactivar' : 'activar';
    if (confirm(`¿Seguro que deseas ${accion} a ${usuario.nombre}?`)) {
      this.usuarioService.toggleActivo(usuario.id).subscribe(() => this.cargarUsuarios());
    }
  }

  eliminar(usuario: Usuario) {
    if (confirm(`¿Borrar definitivamente a ${usuario.nombre}?`)) {
      this.usuarioService.deleteUsuario(usuario.id).subscribe(() => this.cargarUsuarios());
    }
  }

  limpiarFormulario() {
    this.datosForm = {
      id: 0,
      nombre: '',
      apellido: '',
      email: '',
      password: '',
      confirmarPassword: '',
      rolId: 0,
    };
  }
}
