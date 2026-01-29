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
  // Definición de variables necesarias para el nuevo HTML
  email: string = '';
  cargando: boolean = false;

  enviarSolicitud() {
    if (!this.email) return;

    this.cargando = true;

    // Simulación de lógica de envío
    console.log('Enviando código a:', this.email);

    setTimeout(() => {
      this.cargando = false;
      alert('Si el correo existe, recibirás un código pronto.');
    }, 2000);
  }
}
