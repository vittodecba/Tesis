import { Component } from '@angular/core';
import { RouterOutlet, Router } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  title = 'AtonBeerFront';

  // Inyectamos el Router para poder navegar sin recargar la página
  constructor(private router: Router) {}

  logout() {
    localStorage.removeItem('token');
    // Usamos navigate en lugar de window.location
    this.router.navigate(['/login']);
  }
}