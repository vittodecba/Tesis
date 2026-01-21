import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div style="padding: 30px; text-align: center; font-family: sans-serif;">
      <h1>Menú Principal</h1>
      
      <div style="margin: 30px 0; display: flex; gap: 15px; justify-content: center; flex-wrap: wrap;">
        <button routerLink="/usuarios" style="padding: 15px; background: #2980b9; color: white; border: none; border-radius: 5px;">
          Gestionar Usuarios
        </button>
        <button routerLink="/roles" style="padding: 15px; background: #8e44ad; color: white; border: none; border-radius: 5px;">
          Gestionar Roles
        </button>
      </div>

      <hr style="margin: 30px 0;">

      <button (click)="logout()" style="padding: 10px 30px; background: #c0392b; color: white; border: none; border-radius: 5px; cursor: pointer; font-weight: bold;">
        CERRAR SESIÓN
      </button>
    </div>
  `
})
export class DashboardComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}