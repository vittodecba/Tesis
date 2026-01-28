import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-restablecer-contrasena',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './restablecer-contrasena.html',
})
export class RestablecerContrasenaComponent {
  email: string = '';
  token: string = '';
  nuevaPassword: string = '';
  cargando: boolean = false;

  cambiarContrasena(): void {
    if (!this.email || !this.token || !this.nuevaPassword) return;
    this.cargando = true;
    setTimeout(() => {
      this.cargando = false;
      alert('Contraseña cambiada');
    }, 2000);
  }
}
