import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-restablecer-contrasena',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './restablecer-contrasena.html',
})
export class RestablecerContrasenaComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  
  email: string = '';
  token: string = '';
  nuevaPassword: string = '';
  cargando: boolean = false;
  paso: number = 1; 

  enviarSolicitud() {
    if (!this.email) return;
    this.cargando = true;
    this.authService.recuperarContrasena(this.email).subscribe({
      next: () => {
        this.cargando = false;
        this.paso = 2; 
        alert('Código enviado. Revisá tu mail.');
      },
      error: (err: any) => {
        this.cargando = false;
        this.mostrarError(err);
      }
    });
  }

  confirmarRestablecimiento() {
    if (!this.token || !this.nuevaPassword) {
      alert('Completá todos los campos');
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
        alert('Contraseña cambiada con éxito.');
        this.router.navigate(['/login']);
      },
      error: (err: any) => {
        this.cargando = false;
        this.mostrarError(err);
      }
    });
  }

  // MÉTODO NUEVO PARA EXTRAER EL ERROR REAL
  private mostrarError(err: any) {
    let mensaje = 'Ocurrió un error inesperado';

    if (err.error) {
      // 1. Si el backend mandó { "message": "..." }
      if (err.error.message) {
        mensaje = err.error.message;
      } 
      // 2. Si es el formato estándar de validación de .NET { "errors": { "Campo": ["Error"] } }
      else if (err.error.errors) {
        const listaErrores = Object.values(err.error.errors).flat();
        mensaje = listaErrores.join(' ');
      }
      // 3. Si el error es un string simple
      else if (typeof err.error === 'string') {
        mensaje = err.error;
      }
    }

    alert(mensaje);
  }
}