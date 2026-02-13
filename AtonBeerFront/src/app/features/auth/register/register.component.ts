import { Component, inject, OnInit } from '@angular/core'; // Añadido OnInit
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { RolService, RolDto } from '../../../services/rol'; // Importamos tu servicio de roles
import Swal from 'sweetalert2';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html'
})
export class RegisterComponent implements OnInit { // Implementamos OnInit
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private rolService = inject(RolService); // Inyectamos el servicio de roles
  private router = inject(Router);

  registerForm: FormGroup;
  isLoading = false;
  roles: RolDto[] = []; // Ahora es un array vacío que se llenará de la DB

  constructor() {
    this.registerForm = this.fb.group({
      nombre: ['', [Validators.required, Validators.minLength(2)]],
      apellido: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      contrasena: ['', [Validators.required, Validators.minLength(6)]],
      confirmarContrasena: ['', [Validators.required]],
      rolId: [null, [Validators.required]] // Empezamos en null para forzar selección
    }, {
      validators: this.passwordMatchValidator
    });
  }

  ngOnInit(): void {
    this.cargarRoles(); // Cargamos los roles al arrancar
  }

  cargarRoles(): void {
    this.rolService.getRoles().subscribe({
      next: (data) => {
        this.roles = data;
        // Opcional: Si quieres que por defecto seleccione el primer rol que encuentre
        if (this.roles.length > 0) {
          this.registerForm.patchValue({ rolId: this.roles[0].id });
        }
      },
      error: (err) => console.error('Error cargando roles dinámicos:', err)
    });
  }

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('contrasena');
    const confirmPassword = control.get('confirmarContrasena');
    if (!password || !confirmPassword) return null;
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

    this.isLoading = true;
    const formValues = this.registerForm.value;

    const usuarioDto = {
      Nombre: formValues.nombre,
      Apellido: formValues.apellido,
      Email: formValues.email,
      password: formValues.contrasena,
      confirmarPassword: formValues.confirmarContrasena,
      RolId: Number(formValues.rolId)
    };

    this.authService.register(usuarioDto as any).subscribe({
      next: () => {
        this.isLoading = false;
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
        Swal.fire({
          icon: 'error',
          title: 'Error en el registro',
          text: error.error?.message || 'Revisa los datos ingresados.',
          confirmButtonColor: '#E67E22'
        });
      }
    });
  }
}