import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const roles: string[] = route.data['roles'] ?? [];

  if (!auth.isAuthenticated()) {
    router.navigate(['/login']);
    return false;
  }

  if (roles.length === 0 || auth.hasRole(...roles)) return true;

  router.navigate(['/inicio']);
  return false;
};
