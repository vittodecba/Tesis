import { Component, inject, OnInit } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterModule],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);

  usuario: any; //Defino la variable sin asignarle valor todavía

  ngOnInit(): void {
    //Cargo el usuario apenas inicia el componente
    this.usuario = this.authService.getCurrentUser();
  }
  logout(): void {
    Swal.fire({
      title: '¿Cerrar sesión?',
      text: "¡Esperamos verte pronto en Aton Beer!",
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#E67E22',
      cancelButtonColor: '#4A2C2A',
      confirmButtonText: 'Sí, salir',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.authService.logout();
        this.router.navigate(['/login']);
      }
    });
  }
}