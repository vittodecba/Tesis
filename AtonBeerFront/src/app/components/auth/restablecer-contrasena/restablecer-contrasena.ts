import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router'; // Arregla advertencia NG8113

@Component({
  selector: 'app-restablecer-contrasena',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './restablecer-contrasena.html',
})
export class RestablecerContrasenaComponent {
  email: string = '';
  nuevaPassword: string = ''; // Arregla error TS2339 del ngModel
  cargando: boolean = false;

  enviarLink(): void {
    this.cargando = true;
    console.log('Procesando para:', this.email);
    setTimeout(() => (this.cargando = false), 2000);
  }
}
