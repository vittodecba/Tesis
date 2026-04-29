import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { LucideAngularModule, Eye, EyeOff } from 'lucide-angular';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-restablecer-contrasena',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, LucideAngularModule],
  templateUrl: './restablecer-contrasena.html',
})
export class RestablecerContrasenaComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  readonly Eye = Eye;
  readonly EyeOff = EyeOff;

  email: string = '';
  token: string = '';
  nuevaPassword: string = '';
  confirmarPassword: string = '';
  cargando: boolean = false;

  mostrarPassword = false;
  mostrarConfirmarPassword = false;

  confirmarRestablecimiento() {
    if (!this.email || !this.token || !this.nuevaPassword || !this.confirmarPassword) {
      Swal.fire('Error', 'Por favor, completá todos los campos', 'error');
      return;
    }

    if (this.nuevaPassword !== this.confirmarPassword) {
      Swal.fire('Error', 'Las contraseñas no coinciden', 'error');
      return;
    }

    const datos = {
      email: this.email,
      token: this.token,
      nuevaPassword: this.nuevaPassword
    };

    this.cargando = true;
    this.authService.restablecerContrasena(datos).subscribe({
      next: () => {
        this.cargando = false;
        Swal.fire('¡Éxito!', 'Tu contraseña ha sido actualizada', 'success');
        this.router.navigate(['/login']);
      },
      error: (err: any) => {
        this.cargando = false;
        this.mostrarError(err);
      }
    });
  }

  toggleMostrarPassword() {
    this.mostrarPassword = !this.mostrarPassword;
  }

  toggleMostrarConfirmarPassword() {
    this.mostrarConfirmarPassword = !this.mostrarConfirmarPassword;
  }

  private mostrarError(err: any) {
    let mensaje = 'Ocurrió un error inesperado';
    if (err.error) {
      if (err.error.message) mensaje = err.error.message;
      else if (err.error.errors) {
        const listaErrores = Object.values(err.error.errors).flat();
        mensaje = listaErrores.join(' ');
      }
      else if (typeof err.error === 'string') mensaje = err.error;
    }
    Swal.fire('Error', mensaje, 'error');
  }
}