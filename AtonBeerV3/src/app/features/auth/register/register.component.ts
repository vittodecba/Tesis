import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

// 👇 CORRECCIÓN 1: Apuntamos al servicio correcto (3 niveles arriba)

import Swal from 'sweetalert2';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  registerForm: FormGroup;
  isLoading = false;

  roles = [
    { id: 1, nombre: 'Administrador' },
    { id: 2, nombre: 'Gerente' },
    { id: 3, nombre: 'Cocinero' }
  ];

  constructor() {
    this.registerForm = this.fb.group({
      nombre: ['', [Validators.required, Validators.minLength(2)]],
      apellido: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      contrasena: ['', [Validators.required, Validators.minLength(6)]],
      confirmarContrasena: ['', [Validators.required]],
      rolId: [1, [Validators.required]]
    }, {
      validators: this.passwordMatchValidator
    });
  }

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('contrasena');
    const confirmPassword = control.get('confirmarContrasena');

    if (!password || !confirmPassword) {
      return null;
    }

    return password.value === confirmPassword.value ? null : { passwordMismatch: true };
  }

  getFieldError(fieldName: string): string {
    const field = this.registerForm.get(fieldName);
    
    if (field?.hasError('required')) return 'Este campo es obligatorio';
    if (field?.hasError('email')) return 'Ingrese un email válido';
    if (field?.hasError('minlength')) {
      const minLength = field.errors?.['minlength'].requiredLength;
      return `Mínimo ${minLength} caracteres`;
    }
    return '';
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    if (this.registerForm.hasError('passwordMismatch')) {
      Swal.fire({
        icon: 'error',
        title: 'Error',
        text: 'Las contraseñas no coinciden',
        confirmButtonColor: '#E67E22'
      });
      return;
    }

    this.isLoading = true;
    const formValues = this.registerForm.value;

    // --- TRADUCCIÓN: De Angular a C# ---
    const usuarioDto = {
      Nombre: formValues.nombre,
      Apellido: formValues.apellido,
      Email: formValues.email,
      // 👇 CORRECCIÓN 2: Corregí "Pasword" a "Password" (o como lo tenga tu C#)
      Password: formValues.contrasena,           
      ConfirmarPassword: formValues.confirmarContrasena, 
      RolId: Number(formValues.rolId)
    };

    console.log('Enviando paquete a C#:', usuarioDto);

    this.authService.register(usuarioDto as any).subscribe({
      next: () => {
        Swal.fire({
          icon: 'success',
          title: '¡Registro exitoso!',
          text: 'Tu cuenta ha sido creada correctamente',
          confirmButtonColor: '#E67E22'
        }).then(() => {
          this.router.navigate(['/login']);
        });
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Error del Back:', error);
        Swal.fire({
          icon: 'error',
          title: 'Error en el registro',
          text: error.error?.message || 'Revisa que la contraseña tenga mayúscula, número y símbolo.',
          confirmButtonColor: '#E67E22'
        });
      }
    });
  }
}