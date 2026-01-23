import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  loginForm: FormGroup;
  isLoading = false;

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      contrasena: ['', [Validators.required]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;

    this.authService.login(this.loginForm.value).subscribe({
      next: (response) => {
        this.isLoading = false;
        
        // 1. PRIMERO verificamos el Rol
        const rol = response.usuario.rolId;

        if (rol !== 1) {
          // Si NO es admin, tiramos el error DIRECTAMENTE y cortamos acá
          console.warn('Usuario no es admin, bloqueando acceso.');
          Swal.fire({
            icon: 'error',
            title: 'Acceso Denegado',
            text: 'Tu usuario no tiene permisos de administrador para entrar al sistema.',
            confirmButtonColor: '#E67E22'
          });
          
          // Limpiamos la sesión para que no quede nada guardado
          this.authService.logout(); 
          return; // El return hace que NO se ejecute nada de lo que sigue abajo
        }

        // 2. SI ES ADMIN, recién ahora mostramos el éxito y navegamos
        Swal.fire({
          icon: 'success',
          title: '¡Bienvenido!',
          text: `Hola ${response.usuario.nombre}`,
          timer: 1500,
          showConfirmButton: false
        }).then(() => {
          this.router.navigate(['/dashboard/historial']);
        });
      },
      error: (error) => {
        this.isLoading = false;
        Swal.fire({
          icon: 'error',
          title: 'Error de autenticación',
          text: 'Credenciales incorrectas',
          confirmButtonColor: '#E67E22'
        });
      }
    });
  }
}