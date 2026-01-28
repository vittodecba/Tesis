import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterOutlet, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html' 
})
export class DashboardComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  usuario = this.authService.getCurrentUser();
  logout() {
    this.authService.logout();
    // El router.navigate ya lo hace el servicio, pero por las dudas:
    // this.router.navigate(['/login']); 
  }
}