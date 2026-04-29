import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-recuperar-contrasena',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './recuperar-contrasena.html',
})
export class RecuperarContrasenaComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  email: string = '';
  cargando: boolean = false;

  enviarSolicitud(): void {
    if (!this.email) {
      Swal.fire('Atención', 'Por favor ingresá tu email', 'warning');
      return;
    }

    this.cargando = true;
    this.authService.recuperarContrasena(this.email).subscribe({
      next: () => {
        this.cargando = false;
        Swal.fire('Enviado', 'Revisá tu correo para obtener el código de seguridad', 'success');
        this.router.navigate(['/restablecer-contrasena']);
      },
      error: (err) => {
        this.cargando = false;
        const mensaje = err.error?.message || err.error || 'No se pudo enviar el correo';
        Swal.fire('Error', mensaje, 'error');
      },
    });
  }
}