import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

// Componentes de la sección Auth
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { RecuperarContrasenaComponent } from './components/auth/recuperar-contrasena/recuperar-contrasena';

// Componente Dashboard (Tuyo)
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { HistorialComponent } from './components/historial/historial.component';

// Componentes de tus compañeros (Estructura Layout/Inicio)
import { LayoutComponent } from './components/layout/layout.component';
import { InicioComponent } from './components/inicio/inicio.component';
import { UsuariosComponent } from './components/usuarios/usuarios';
import { ClientesComponent } from './components/clientes/clientes.component';
import { RolesGestion } from './components/roles-gestion/roles-gestion';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'registro', component: RegisterComponent },
  { path: 'recuperar-password', component: RecuperarContrasenaComponent },
  
  // TU SECCIÓN: Dashboard propio para Historial
  { 
    path: 'dashboard', 
    component: DashboardComponent, 
    canActivate: [authGuard], 
    children: [
      { path: 'historial', component: HistorialComponent },
      { path: '', redirectTo: 'historial', pathMatch: 'full' }
    ]
  },

  // SECCIÓN COMPAÑEROS: Layout para Inicio, Clientes, etc.
  {
    path: 'sistema', 
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'inicio', component: InicioComponent },
      { path: 'clientes', component: ClientesComponent },
      { path: 'usuarios', component: UsuariosComponent },
      { path: 'roles', component: RolesGestion },
    ],
  },

  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];