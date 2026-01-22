import { Routes } from '@angular/router';
import { ClientesComponent } from './components/clientes/clientes.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';

// --- IMPORTACIONES CORREGIDAS ---
import { UsuariosComponent } from './components/usuarios/usuarios';
import { RolesGestion } from './components/roles-gestion/roles-gestion'; // <--- Corregido: Clase "RolesGestion"
import { RecuperarContrasenaComponent } from './components/auth/recuperar-contrasena/recuperar-contrasena';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'registro', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'clientes', component: ClientesComponent },

  { path: 'usuarios', component: UsuariosComponent },
  { path: 'roles', component: RolesGestion }, // <--- Usando la clase correcta
  { path: 'recuperar-password', component: RecuperarContrasenaComponent },

  { path: '', redirectTo: 'login', pathMatch: 'full' },
];
