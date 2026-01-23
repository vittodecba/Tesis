import { Routes } from '@angular/router';
import { authGuard, adminGuard } from './core/guards/auth.guards';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { 
    path: 'login', 
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent) 
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/auth/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' }, // Redirige a Home por defecto
      { 
        path: 'home', 
        loadComponent: () => import('./features/auth/dashboard/home/home.component').then(m => m.HomeComponent) 
      },
      { 
        path: 'historial', 
        loadComponent: () => import('./features/auth/dashboard/historial/historial.component').then(m => m.HistorialComponent),
        canActivate: [adminGuard]
      }
    ]
  },
  { path: '**', redirectTo: 'login' }
];