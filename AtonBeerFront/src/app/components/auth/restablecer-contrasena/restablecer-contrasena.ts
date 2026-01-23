import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-restablecer-contrasena',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './restablecer-contrasena.html',
  styleUrl: './restablecer-contrasena.css'
})
export class RestablecerContrasenaComponent {
  email: string = '';
  token: string = '';
  nuevaPassword: string = '';
  cargando: boolean = false;

  constructor(private authService: AuthService, private router: Router) {}

  cambiarContrasena() {
    if (!this.email || !this.token || !this.nuevaPassword) {
      Swal.fire('Error', 'Por favor completá todos los campos.', 'error');
      return;
    }

    this.cargando = true;

    const datos = {
      email: this.email,
      token: this.token,
      nuevaPassword: this.nuevaPassword
    };

    this.authService.restablecerContrasena(datos).subscribe({
      next: (response) => {
        this.cargando = false;
        Swal.fire({
          title: '¡Éxito!',
          text: 'Tu contraseña fue actualizada. Ya podés iniciar sesión.',
          icon: 'success',
          confirmButtonText: 'Ir al Login'
        }).then(() => {
          this.router.navigate(['/']); 
        });
      },
      error: (error) => {
        this.cargando = false;
        Swal.fire('Error', error.error.message || 'El token es inválido o expiró.', 'error');
      }
    });
  }
}