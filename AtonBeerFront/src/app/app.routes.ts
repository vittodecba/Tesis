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
        data: { title: 'Stock', subtitle: 'Control de productos' },
      },
      {
        path: 'insumos',
        component: InsumoComponent,
        data: { title: 'Insumos', subtitle: 'Gestión de materia prima' },
      },
      {
        path: 'unidades-medida',
        component: UnidadMedidaComponent,
        data: { title: 'Unidades', subtitle: 'Gestión de medidas' },
      },
      {
        path: 'fermentadores',
        component: FermentadorComponent,
        data: { title: 'Fermentadores', subtitle: 'Gestión de tanques de fermentación' },
      },
      {
        path: 'fermentadores/:id',
        component: FermentadorDetalleComponent,
        data: { title: 'Detalle de Fermentador', subtitle: 'Seguimiento del lote y fermentación' },
      },
      {
        path: 'recetas',
        component: RecetaListComponent,
        data: { title: 'Recetas', subtitle: 'Gestión de recetas de cerveza' },
      },
      {
        path: 'recetas/detalle/:id',
        component: RecetaDetalle,
        data: { title: 'Detalle de Receta', subtitle: 'Información completa' },
      },
      {
        path: 'planificacion',
        component: LoteListadoComponent,
        data: { title: 'Planificación', subtitle: 'Gestión y seguimiento de lotes' },
      },
      {
        path: 'planificacion/Listado',
        component: PlanificacionListComponent,
        data: { title: 'Planificacion', subtitle: 'Planificaciones activas' }
      },
      {
        path: 'planificacion/nueva',
        component: PlanificacionFormComponent,
        data: { title: 'Nueva Planificacion', subtitle: 'Programar Coccion' }
      },
      {
        path: 'planificacion/detalle/:id',
        component: LoteDetalleComponent,
        data: { title: 'Detalle de Lote', subtitle: 'Información completa del lote' },
      },
      {
        path: 'pedidos/registrar',
        component: RegistrarPedidoComponent,
        data: { title: 'Pedidos', subtitle: 'Administración y registro de pedidos' }
      },
      {
        path: 'barriles',
        component: BarrilesGestion,
        data: { title: 'Barriles', subtitle: 'Seguimiento de activos retornables' }
      },      
      {
        path: 'barriles/:id',
        component: BarrilDetalleComponent,
        data: { title: 'Detalle de Barril', subtitle: 'Información y movimientos' },
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