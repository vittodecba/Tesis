import { Routes } from '@angular/router';
import { LayoutComponent } from './components/layout/layout';
import { LoginComponent } from './features/auth/login/login.component';
import { RecuperarContrasenaComponent } from './components/auth/recuperar-contrasena/recuperar-contrasena';
import { RestablecerContrasenaComponent } from './components/auth/restablecer-contrasena/restablecer-contrasena';
import { ClientesComponent } from './components/clientes/clientes.component';
import { InicioComponent } from './components/Inicio/inicio.component';
import { UsuariosComponent } from './components/usuarios/usuarios';
import { FermentadorDetalleComponent } from './components/Fermentador-detalle/fermentador-detalle';
import { RolesGestion } from './components/roles-gestion/roles-gestion';
import { StockGestion } from './components/stock/stock-gestion';
import { InsumoComponent } from './components/insumo/insumo';
import { UnidadMedidaComponent } from './components/unidadesMedida/unidadMedidaComponent';
import { RecetaListComponent } from './components/recetas/receta-list/receta-list';
import { RecetaDetalle } from './components/recetas/receta-detalle/receta-detalle';
import { LoteDetalleComponent } from './components/lote-detalle/lote-detalle';
import { LoteListadoComponent } from './components/lote-listado/lote-listado';
import { FermentadorComponent } from './components/fermentador/fermentador';
import { PlanificacionListComponent } from './components/Planificacion/PlanificacionListado/PlanListado';
import { PlanificacionFormComponent } from './components/Planificacion/PlanificacionLotes/PlanificacionComponent';
import { RegistrarPedidoComponent } from './components/registrar-pedido/registrar-pedido';
import { BarrilesGestion } from './components/barriles/barriles-gestion';
import { BarrilDetalleComponent } from './components/barriles/barril-detalle/barril-detalle';
import { VentasComponent } from './components/ventas/ventas.component';
import { HistorialAccesoComponent } from './components/historial/historialComponent';
import { roleGuard } from './core/guards/role.guard';
import { ROLES } from './core/constants/roles';

const A = ROLES.ADMIN;
const G = ROLES.GERENTE;
const GM = ROLES.GERENTE_MAYOR;
const RP = ROLES.RESP_PLANTA;
const RPE = ROLES.RESP_PEDIDOS;
const C = ROLES.COCINERO;
import { VentasListadoComponent } from './components/ventas/ventas-listado';
import { VentasReporte } from './components/ventas-reporte/ventas-reporte';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'recuperar-password', component: RecuperarContrasenaComponent },
  { path: 'restablecer-contrasena', component: RestablecerContrasenaComponent },
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
        canActivate: [roleGuard],
        data: { title: 'Clientes', subtitle: 'Gestión de franquicias', roles: [A, G, RPE] },
      },
      {
        path: 'usuarios',
        component: UsuariosComponent,
        canActivate: [roleGuard],
        data: { title: 'Usuarios', subtitle: 'Administración de accesos', roles: [A, G] },
      },
      {
        path: 'roles',
        component: RolesGestion,
        canActivate: [roleGuard],
        data: { title: 'Roles', subtitle: 'Permisos del sistema', roles: [A, G] },
      },
      {
        path: 'historial-accesos',
        component: HistorialAccesoComponent,
        canActivate: [roleGuard],
        data: { title: 'Historial de Accesos', subtitle: 'Auditoría de inicios de sesión', roles: [A, G] },
      },
      {
        path: 'stock',
        component: StockGestion,
        canActivate: [roleGuard],
        data: { title: 'Stock', subtitle: 'Control de productos', roles: [A, RP, C] },
      },
      {
        path: 'insumos',
        component: InsumoComponent,
        canActivate: [roleGuard],
        data: { title: 'Insumos', subtitle: 'Gestión de materia prima', roles: [A, RP, C] },
      },
      {
        path: 'unidades-medida',
        component: UnidadMedidaComponent,
        canActivate: [roleGuard],
        data: { title: 'Unidades', subtitle: 'Gestión de medidas', roles: [A, RP] },
      },
      {
        path: 'fermentadores',
        component: FermentadorComponent,
        canActivate: [roleGuard],
        data: { title: 'Fermentadores', subtitle: 'Gestión de tanques de fermentación', roles: [A, RP, C] },
      },
      {
        path: 'fermentadores/:id',
        component: FermentadorDetalleComponent,
        canActivate: [roleGuard],
        data: { title: 'Detalle de Fermentador', subtitle: 'Seguimiento del lote y fermentación', roles: [A, RP, C] },
      },
      {
        path: 'recetas',
        component: RecetaListComponent,
        canActivate: [roleGuard],
        data: { title: 'Recetas', subtitle: 'Gestión de recetas de cerveza', roles: [A, RP, C] },
      },
      {
        path: 'recetas/detalle/:id',
        component: RecetaDetalle,
        canActivate: [roleGuard],
        data: { title: 'Detalle de Receta', subtitle: 'Información completa', roles: [A, RP, C] },
      },
      {
        path: 'planificacion',
        component: LoteListadoComponent,
        canActivate: [roleGuard],
        data: { title: 'Planificación', subtitle: 'Gestión y seguimiento de lotes', roles: [A, RP] },
      },
      {
        path: 'planificacion/Listado',
        component: PlanificacionListComponent,
        canActivate: [roleGuard],
        data: { title: 'Planificacion', subtitle: 'Planificaciones activas', roles: [A, RP] },
      },
      {
        path: 'planificacion/nueva',
        component: PlanificacionFormComponent,
        canActivate: [roleGuard],
        data: { title: 'Nueva Planificacion', subtitle: 'Programar Coccion', roles: [A, RP] },
      },
      {
        path: 'planificacion/detalle/:id',
        component: LoteDetalleComponent,
        canActivate: [roleGuard],
        data: { title: 'Detalle de Lote', subtitle: 'Información completa del lote', roles: [A, RP] },
      },
      {
        path: 'pedidos/registrar',
        component: RegistrarPedidoComponent,
        canActivate: [roleGuard],
        data: { title: 'Pedidos', subtitle: 'Administración y registro de pedidos', roles: [A, G, RP, RPE] },
        data: { title: 'Pedidos', subtitle: 'Administración y registro de pedidos' }
      },
      {
        path: 'barriles',
        component: BarrilesGestion,
        canActivate: [roleGuard],
        data: { title: 'Barriles', subtitle: 'Seguimiento de activos retornables', roles: [A, RP, RPE] },
      },
      {
        path: 'barriles/:id',
        component: BarrilDetalleComponent,
        canActivate: [roleGuard],
        data: { title: 'Detalle de Barril', subtitle: 'Información y movimientos', roles: [A, RP, RPE] },
      },
      {
        path: 'ventas',
        component: VentasComponent,
        canActivate: [roleGuard],
        data: { title: 'Ventas', subtitle: 'Registro de ventas generadas', roles: [A, G, GM] },
      },
      {
        path: 'ventas/reporte',
        component: VentasReporte,
        data: { title: 'Reportes de Ventas', subtitle: 'Análisis financiero' },
      },
      {
        path: 'ventas',
        component: VentasListadoComponent,
        data: { title: 'Ventas', subtitle: 'Gestión de ventas' },
      },
    ],
  },
  { path: '**', redirectTo: 'login' },
];
