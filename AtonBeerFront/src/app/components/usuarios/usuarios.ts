import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Pencil, Ban, CheckCircle, Eye, EyeOff, Trash2, AlertTriangle } from 'lucide-angular';
import { UsuarioService } from '../../services/usuario.service';
import { RolService } from '../../services/rol';
import { NotificationService } from '../../core/services/notification.service';
import { Usuario, UsuarioCreate, UsuarioUpdate } from '../../Interfaces/usuario.interface';

@Component({
  selector: 'app-usuarios',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './usuarios.html',
  styleUrls: ['./usuarios.css'],
})
export class UsuariosComponent implements OnInit {
  readonly Pencil = Pencil;
  readonly Ban = Ban;
  readonly CheckCircle = CheckCircle;
  readonly Eye = Eye;
  readonly EyeOff = EyeOff;
  readonly Trash2 = Trash2;
  readonly AlertTriangle = AlertTriangle;

  usuarios: Usuario[] = [];
  roles: any[] = [];
  mostrarModal: boolean = false;
  mostrarModalEliminar: boolean = false;
  usuarioAEliminar: Usuario | null = null;
  esEdicion: boolean = false;
  tituloModal: string = 'Nuevo Usuario';
  verInactivos: boolean = false;

  mostrarPassword = false;
  mostrarConfirmarPassword = false;

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
    private noti: NotificationService,
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

  toggleVerInactivos() {
    this.cargarUsuarios();
  }

  abrirModal(usuario?: Usuario) {
    this.mostrarModal = true;
    this.mostrarPassword = false;
    this.mostrarConfirmarPassword = false;
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
        rolId: usuario.rolId,
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

  toggleMostrarPassword() {
    this.mostrarPassword = !this.mostrarPassword;
  }

  toggleMostrarConfirmarPassword() {
    this.mostrarConfirmarPassword = !this.mostrarConfirmarPassword;
  }

  guardar() {
    if (!this.datosForm.nombre.trim() || !this.datosForm.apellido.trim() || !this.datosForm.email.trim() || !this.datosForm.rolId) {
      this.noti.warning('Por favor, completá los campos obligatorios: Nombre, Apellido, Email y Rol.');
      return;
    }

    // Validación de formato de email: un único mensaje claro en vez de mezclar los
    // distintos errores que devuelve el backend (formato + contraseña, etc.).
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(this.datosForm.email.trim())) {
      this.noti.warning('Email inválido');
      return;
    }

    const emailIngresado = this.datosForm.email.trim();
    const emailDuplicado = this.usuarios.some(u => 
      u.email.toLowerCase() === emailIngresado.toLowerCase() && 
      u.id !== this.datosForm.id
    );

    if (emailDuplicado) {
      this.noti.warning('Ese email ya está registrado. Por favor, ingresá uno distinto.');
      return;
    }

    if (this.esEdicion) {
      const dto: UsuarioUpdate = {
        id: this.datosForm.id,
        nombre: this.datosForm.nombre,
        apellido: this.datosForm.apellido,
        email: this.datosForm.email,
        rolId: Number(this.datosForm.rolId),
        activo: true, 
      };

      this.usuarioService.updateUsuario(this.datosForm.id, dto).subscribe({
        next: () => {
          this.noti.success('¡Usuario modificado!');
          this.cerrarModal();
          this.cargarUsuarios();
        },
        error: (e) => {
          let mensaje = 'Error al modificar el usuario.';
          if (typeof e.error === 'string') mensaje = e.error;
          else if (e.error?.errors) mensaje = Object.values(e.error.errors).flat().join('\n');
          else if (e.error?.message) mensaje = e.error.message;
          this.noti.error(mensaje);
        }
      });
    } else {
      if (this.datosForm.password !== this.datosForm.confirmarPassword) {
        this.noti.warning('Las contraseñas no coinciden');
        return;
      }
      const dto: UsuarioCreate = { ...this.datosForm, rolId: Number(this.datosForm.rolId) };
      this.usuarioService.createUsuario(dto).subscribe({
        next: () => {
          this.noti.success('¡Usuario creado!');
          this.cerrarModal();
          this.cargarUsuarios();
        },
        error: (e) => {
          let mensaje = 'Error al crear el usuario.';
          if (typeof e.error === 'string') mensaje = e.error;
          else if (e.error?.errors) mensaje = Object.values(e.error.errors).flat().join('\n');
          else if (e.error?.message) mensaje = e.error.message;
          this.noti.error(mensaje);
        }
      });
    }
  }

  async toggleActivo(usuario: Usuario) {
    const userJson = localStorage.getItem('aton_user');
    const idLogueado = userJson ? JSON.parse(userJson).id : null;

    if (usuario.id === idLogueado) {
      this.noti.warning('No podés desactivar tu propia cuenta mientras estás en sesión.');
      return;
    }

    const accion = usuario.activo ? 'desactivar' : 'activar';
    const estadoFinal = usuario.activo ? 'desactivado' : 'activado';

    const ok = await this.noti.confirm({
      titulo: `¿${accion.charAt(0).toUpperCase() + accion.slice(1)} usuario?`,
      texto: `Se va a ${accion} a ${usuario.nombre}.`,
      peligro: usuario.activo
    });
    if (ok) {
      this.usuarioService.toggleActivo(usuario.id).subscribe({
        next: () => {
          this.noti.success(`Usuario ${estadoFinal} con éxito`);
          this.cargarUsuarios();
        },
        error: (err) => this.noti.error('Error al cambiar el estado del usuario.')
      });
    }
  }

  abrirModalEliminar(usuario: Usuario) {
    const userJson = localStorage.getItem('aton_user');
    const idLogueado = userJson ? JSON.parse(userJson).id : null;

    if (usuario.id === idLogueado) {
      this.noti.warning('No podés eliminar tu propia cuenta mientras estás en sesión.');
      return;
    }

    this.usuarioAEliminar = usuario;
    this.mostrarModalEliminar = true;
  }

  cerrarModalEliminar() {
    this.mostrarModalEliminar = false;
    this.usuarioAEliminar = null;
  }

  confirmarEliminar() {
    if (!this.usuarioAEliminar) return;

    this.usuarioService.eliminarPermanente(this.usuarioAEliminar.id).subscribe({
      next: () => {
        this.noti.success('Usuario eliminado permanentemente.');
        this.cerrarModalEliminar();
        this.cargarUsuarios();
      },
      error: (e) => {
        let mensaje = 'No se pudo eliminar el usuario.';
        if (typeof e.error === 'string') mensaje = e.error;
        else if (e.error?.message) mensaje = e.error.message;
        this.noti.error(mensaje);
      }
    });
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