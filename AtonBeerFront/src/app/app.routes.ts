import { Routes } from '@angular/router';
import { LayoutComponent } from './components/layout/layout';
import { LoginComponent } from './features/auth/login/login.component';
import { ClientesComponent } from './components/clientes/clientes.component';
import { InicioComponent } from './components/Inicio/inicio.component';
import { UsuariosComponent } from './components/usuarios/usuarios';
import { RolesGestion } from './components/roles-gestion/roles-gestion';

export const routes: Routes = [
  // ESTO ES LO MÁS IMPORTANTE: La raíz redirige a login, no a inicio.
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
        data: { title: 'Clientes', subtitle: 'Gestión de franquicias' },
      },
      {
        path: 'usuarios',
        component: UsuariosComponent,
        data: { title: 'Usuarios', subtitle: 'Administración de accesos' },
      },
      {
        path: 'roles',
        component: RolesGestion,
        data: { title: 'Roles', subtitle: 'Permisos del sistema' },
      },
    ],
  },
  { path: '**', redirectTo: 'login' },
];
