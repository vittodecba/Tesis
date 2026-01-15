import { Routes } from '@angular/router';
import { RolesGestion } from './components/roles-gestion/roles-gestion';
import { UsuariosComponent } from './components/usuarios/usuarios';

export const routes: Routes = [
    { path: 'roles-gestion', component: RolesGestion },
    { path: 'usuarios', component: UsuariosComponent }
];