import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import Swal from 'sweetalert2';

//Verifica que el usuario esté logueado
export const authGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};

//Verifica que esté logueado Y que sea ADMIN
export const adminGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const usuario = authService.getCurrentUser();

  if (authService.isAuthenticated() && usuario?.rolId === 1) {
    return true;
  }

  // Si no es admin, lo mandamos a una ruta que NO tenga adminGuard
  // Importante: No lo mandes a /dashboard si el dashboard por defecto abre el historial
  Swal.fire({
    title: 'Acceso Denegado',
    text: 'Esta sección es solo para administradores.',
    icon: 'error',
    timer: 2000, // Agregamos timer para que no bloquee el hilo
    showConfirmButton: false
  });
  
  // Lo mandamos al login o a una página neutra
  router.navigate(['/login']); 
  return false;
};