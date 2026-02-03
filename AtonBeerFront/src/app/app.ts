import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet, Router } from '@angular/router';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: '<router-outlet></router-outlet>',
})
export class App implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);

  // Agregamos el título que el test .spec busca
  title = 'AtonBeerFront';

  ngOnInit() {
    // Si no está autenticado, lo mandamos al login apenas abre la app
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
    }
  }

  // Función de logout unificada con el servicio
  logout() {
    this.authService.logout();
  }
}
