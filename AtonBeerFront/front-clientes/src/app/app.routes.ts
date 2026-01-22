import { Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout';
import { Clientes } from './pages/clientes/clientes';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', redirectTo: 'clientes', pathMatch: 'full' },

      {
        path: 'clientes',
        component: Clientes,
        data: { title: 'Clientes', subtitle: 'Gestiona franquicias y clientes externos' },
      },

      // placeholders por ahora
      {
        path: 'inicio',
        component: Clientes,
        data: { title: 'Inicio', subtitle: 'Panel de control principal' },
      },
      {
        path: 'insumos',
        component: Clientes,
        data: { title: 'Insumos', subtitle: 'Gestión de insumos y proveedores' },
      },
      {
        path: 'recetas',
        component: Clientes,
        data: { title: 'Recetas', subtitle: 'Gestión de recetas y fórmulas' },
      },
      {
        path: 'planificacion',
        component: Clientes,
        data: { title: 'Planificación', subtitle: 'Planificación de producción' },
      },
      {
        path: 'fermentacion',
        component: Clientes,
        data: { title: 'Fermentación', subtitle: 'Control del proceso de fermentación' },
      },
      {
        path: 'stock',
        component: Clientes,
        data: { title: 'Stock', subtitle: 'Control y movimientos de stock' },
      },
      {
        path: 'pedidos',
        component: Clientes,
        data: { title: 'Pedidos', subtitle: 'Gestión de pedidos' },
      },
      {
        path: 'barriles',
        component: Clientes,
        data: { title: 'Seguimiento Barriles', subtitle: 'Trazabilidad y estado de barriles' },
      },
      {
        path: 'ventas',
        component: Clientes,
        data: { title: 'Ventas', subtitle: 'Registro y seguimiento de ventas' },
      },
      {
        path: 'reportes',
        component: Clientes,
        data: { title: 'Reportes de Ventas', subtitle: 'Indicadores y reportes' },
      },
    ],
  },
];
