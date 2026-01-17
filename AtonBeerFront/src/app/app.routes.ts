import { Routes } from '@angular/router';
import { RolesGestion } from './components/roles-gestion/roles-gestion';
import { UsuariosComponent } from './components/usuarios/usuarios';
import { RecuperarContrasenaComponent } from './components/auth/recuperar-contrasena/recuperar-contrasena'; 
import { RestablecerContrasenaComponent } from './components/auth/restablecer-contrasena/restablecer-contrasena';

export const routes: Routes = [
    { path: 'roles-gestion', component: RolesGestion },
    { path: 'usuarios', component: UsuariosComponent },
    { path: 'recuperar-contrasena', component: RecuperarContrasenaComponent },
    { path: 'restablecer-contrasena', component: RestablecerContrasenaComponent }
];