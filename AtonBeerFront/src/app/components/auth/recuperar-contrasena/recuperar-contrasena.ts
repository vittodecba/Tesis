import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // Para usar ngModel
import { AuthService } from '../../../services/auth.service'; // Tu servicio nuevo
import { Router } from '@angular/router';
import Swal from 'sweetalert2'; // Para alertas lindas

@Component({
  selector: 'app-recuperar-contrasena',
  standalone: true,
  imports: [CommonModule, FormsModule], // Importamos FormsModule acá
  templateUrl: './recuperar-contrasena.html',
  styleUrl: './recuperar-contrasena.css'
})
export class RecuperarContrasenaComponent {
  email: string = '';
  cargando: boolean = false;

  constructor(private authService: AuthService, private router: Router) {}

  enviarSolicitud() {
    if (!this.email) {
      Swal.fire('Error', 'Por favor ingresá tu email.', 'error');
      return;
    }

    this.cargando = true; // Activa el circulito de carga

    this.authService.recuperarContrasena(this.email).subscribe({
      next: (response) => {
        this.cargando = false;
        Swal.fire('¡Enviado!', 'Revisá tu correo, te enviamos un código.', 'success');
        // Acá podríamos redirigir a la pantalla de poner el token, 
        // pero primero vamos a crearla en el siguiente paso.
      },
      error: (error) => {
        this.cargando = false;
        Swal.fire('Error', 'No se pudo enviar el correo. Verificá que el email sea correcto.', 'error');
      }
    });
  }
}