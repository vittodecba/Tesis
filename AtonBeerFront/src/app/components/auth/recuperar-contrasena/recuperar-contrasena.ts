import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-recuperar-contrasena',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './recuperar-contrasena.html',
})
export class RecuperarContrasenaComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  email: string = '';
  cargando: boolean = false;

  enviarSolicitud(): void {
    if (!this.email) return;

    this.cargando = true;
    this.authService.recuperarContrasena(this.email).subscribe({
      next: (response: any) => {
        this.cargando = false;
        Swal.fire('Enviado', 'Revisa tu correo electrónico', 'success');
        this.router.navigate(['/login']);
      },
      error: (err: any) => {
        this.cargando = false;
        Swal.fire('Error', 'No se pudo enviar el correo', 'error');
      },
    });
  }
}
