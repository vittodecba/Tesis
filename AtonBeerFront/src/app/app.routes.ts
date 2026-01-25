import { Routes } from '@angular/router';
import { LayoutComponent } from './components/layout/layout';
import { LoginComponent } from './features/auth/login/login.component';
import { ClientesComponent } from './components/clientes/clientes.component';
import { InicioComponent } from './components/Inicio/inicio.component'; // Importación corregida con I mayúscula
import { UsuariosComponent } from './components/usuarios/usuarios';
import { RolesGestion } from './components/roles-gestion/roles-gestion';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: LayoutComponent,
    children: [
      {
        path: 'inicio',
        component: InicioComponent,
        data: { title: 'Inicio', subtitle: 'Panel de control principal' },
      },
      {
        path: 'clientes',
        component: ClientesComponent,
        data: { title: 'Clientes', subtitle: 'Gestión de franquicias y clientes externos' },
      },
      {
        path: 'usuarios',
        component: UsuariosComponent,
        data: { title: 'Usuarios', subtitle: 'Gestión de accesos al sistema' },
      },
      {
        path: 'roles',
        component: RolesGestion,
        data: { title: 'Roles', subtitle: 'Configuración de permisos' },
      },
    ],
  },
  { path: '**', redirectTo: 'login' },
];
