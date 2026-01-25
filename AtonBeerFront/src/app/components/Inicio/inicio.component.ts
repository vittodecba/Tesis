import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule, Users, Box, ClipboardList, TrendingUp } from 'lucide-angular';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-inicio',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './inicio.component.html',
  styleUrls: ['./inicio.component.scss'],
})
export class InicioComponent implements OnInit {
  private authService = inject(AuthService);

  // Iconos
  Users = Users;
  Box = Box;
  ClipboardList = ClipboardList;
  TrendingUp = TrendingUp;

  currentUser = this.authService.getCurrentUser();
  fechaActual = new Date();

  // Estos son los datos que ves en los cuadritos blancos de tu imagen
  stats = [
    { label: 'Clientes Activos', value: '24', icon: Users, color: 'blue' },
    { label: 'Stock Total', value: '1,250 L', icon: Box, color: 'orange' },
    { label: 'Pedidos Pendientes', value: '8', icon: ClipboardList, color: 'green' },
    { label: 'Ventas del Mes', value: '$450k', icon: TrendingUp, color: 'purple' },
  ];

  ngOnInit(): void {
    // Aquí podrías llamar a un servicio para traer datos reales de la DB
  }
}
