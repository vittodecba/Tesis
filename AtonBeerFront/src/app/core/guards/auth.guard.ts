import { inject } from '@angular/core';
import { Router } from '@angular/router';
// CORRECCIÓN: Subimos 2 niveles para buscar el servicio en app/services
import { AuthService } from '../../services/auth.service'; 
import Swal from 'sweetalert2';

export const authGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};

export const adminGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const usuario = authService.getCurrentUser();

  if (authService.isAuthenticated() && usuario?.rolId === 1) {
    return true;
  }

  Swal.fire({
    title: 'Acceso Denegado',
    text: 'No tenés permisos de administrador.',
    icon: 'error',
    timer: 2000,
    showConfirmButton: false
  });
  
  router.navigate(['/dashboard']); 
  return false;
};