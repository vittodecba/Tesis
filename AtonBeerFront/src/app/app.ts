import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
// Importamos el componente de Roles para poder usarlo
import { RolesGestion } from './components/roles-gestion/roles-gestion';

@Component({
  selector: 'app-root',
  standalone: true,
  // Agregamos RolesGestion a la lista de cosas permitidas (imports)
  imports: [RouterOutlet, RolesGestion],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  title = 'AtonBeerFront';
}