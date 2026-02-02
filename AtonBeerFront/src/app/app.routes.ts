import { Routes } from '@angular/router';
import { LayoutComponent } from './components/layout/layout';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { RestablecerContrasenaComponent } from './components/auth/restablecer-contrasena/restablecer-contrasena';
import { ClientesComponent } from './components/clientes/clientes.component';
import { InicioComponent } from './components/Inicio/inicio.component';
import { UsuariosComponent } from './components/usuarios/usuarios';
import { RolesGestion } from './components/roles-gestion/roles-gestion';
import { StockGestion } from './components/stock/stock-gestion'; // 👈 IMPORT NUEVO

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  // 🔐 AUTH (fuera del layout)
  { path: 'login', component: LoginComponent },
  { path: 'registro', component: RegisterComponent },
  { path: 'recuperar-password', component: RestablecerContrasenaComponent },

  // 🧠 APP (con layout)
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
      {
        path: 'stock',
        component: StockGestion,
        data: {
          title: 'Stock',
          subtitle: 'Control de productos, latas y barriles',
        },
      },
    ],
  },

  // fallback
  { path: '**', redirectTo: 'login' },
];
