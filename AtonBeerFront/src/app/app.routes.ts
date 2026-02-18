import { Routes } from '@angular/router';
import { LayoutComponent } from './components/layout/layout';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { RestablecerContrasenaComponent } from './components/auth/restablecer-contrasena/restablecer-contrasena';
import { ClientesComponent } from './components/clientes/clientes.component';
import { InicioComponent } from './components/Inicio/inicio.component';
import { UsuariosComponent } from './components/usuarios/usuarios';
import { RolesGestion } from './components/roles-gestion/roles-gestion';
import { StockGestion } from './components/stock/stock-gestion';
import { InsumoComponent } from './components/insumo/insumo';
import { UnidadMedidaComponent } from './components/unidadesMedida/unidadMedidaComponent';
// Importo los componentes de recetas
import { RecetaListComponent } from './components/recetas/receta-list/receta-list';
import { RecetaDetalle } from './components/recetas/receta-detalle/receta-detalle';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'registro', component: RegisterComponent },
  { path: 'recuperar-password', component: RestablecerContrasenaComponent },

  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: 'inicio', component: InicioComponent, data: { title: 'Inicio', subtitle: 'Panel de control principal' } },
      { path: 'clientes', component: ClientesComponent, data: { title: 'Clientes', subtitle: 'Gestión de franquicias' } },
      { path: 'usuarios', component: UsuariosComponent, data: { title: 'Usuarios', subtitle: 'Administración de accesos' } },
      { path: 'roles', component: RolesGestion, data: { title: 'Roles', subtitle: 'Permisos del sistema' } },
      { path: 'stock', component: StockGestion, data: { title: 'Stock', subtitle: 'Control de productos' } },
      
      { 
        path: 'insumos', 
        component: InsumoComponent, 
        data: { title: 'Insumos', subtitle: 'Gestión de materia prima' } 
      },

      { 
        path: 'unidades-medida', 
        component: UnidadMedidaComponent, 
        data: { title: 'Unidades', subtitle: 'Gestión de medidas' } 
      },

      // --- RUTAS DE RECETAS ---
      { 
        path: 'recetas', 
        component: RecetaListComponent, 
        data: { title: 'Recetas', subtitle: 'Gestión de recetas de cerveza' } 
      },
      { 
        path: 'recetas/detalle/:id', 
        component: RecetaDetalle, 
        data: { title: 'Detalle de Receta', subtitle: 'Información completa' } 
      },
    ],
  },
  { path: '**', redirectTo: 'login' },
];