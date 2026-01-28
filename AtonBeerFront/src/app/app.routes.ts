import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { RecuperarContrasenaComponent } from './components/auth/recuperar-contrasena/recuperar-contrasena';

import { HistorialComponent } from './components/historial/historial.component';
import { UsuariosComponent } from './components/usuarios/usuarios';
import { ClientesComponent } from './components/clientes/clientes.component';
import { RolesGestion } from './components/roles-gestion/roles-gestion';

// CORRECCIÓN: Ruta correcta al guard
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'registro', component: RegisterComponent },
  { path: 'recuperar-password', component: RecuperarContrasenaComponent },
  { 
    path: 'dashboard', 
    component: DashboardComponent,
    canActivate: [authGuard], 
    children: [
      { path: '', redirectTo: 'historial', pathMatch: 'full' },
      { path: 'historial', component: HistorialComponent },
      { path: 'usuarios', component: UsuariosComponent },
      { path: 'clientes', component: ClientesComponent },
      { path: 'roles', component: RolesGestion }
    ]
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];