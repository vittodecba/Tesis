import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ActivatedRoute,
  NavigationEnd,
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { filter } from 'rxjs/operators';

import {
  Beer,
  Home,
  ClipboardList,
  BookOpen,
  CalendarDays,
  FlaskConical,
  Boxes,
  Users,
  Receipt,
  Barrel,
  CreditCard,
  LineChart,
  LogOut,
  User,
} from 'lucide-angular';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet, LucideAngularModule],
  templateUrl: './layout.html',
  styleUrls: ['./layout.scss'],
})
export class LayoutComponent {
  // Iconos
  Beer = Beer;
  Home = Home;
  ClipboardList = ClipboardList;
  BookOpen = BookOpen;
  CalendarDays = CalendarDays;
  FlaskConical = FlaskConical;
  Boxes = Boxes;
  Users = Users;
  Receipt = Receipt;
  Barrel = Barrel;
  CreditCard = CreditCard;
  LineChart = LineChart;
  LogOut = LogOut;
  User = User;

  // Topbar dinámico
  pageTitle = 'Inicio';
  pageSub = 'Panel de control principal';

  constructor(private router: Router, private activatedRoute: ActivatedRoute) {
    // set inicial (seguro)
    this.updateFromRouteSafe();

    // set al navegar (después de que termina)
    this.router.events
      .pipe(filter((e) => e instanceof NavigationEnd))
      .subscribe(() => this.updateFromRouteSafe());
  }

  private updateFromRouteSafe(): void {
    // nos movemos hasta la ruta hija activa
    let route = this.activatedRoute;
    while (route.firstChild) {
      route = route.firstChild;
    }

    // data puede venir vacío en algunas rutas => fallback
    const data = route.snapshot?.data ?? {};

    this.pageTitle = data['title'] ?? 'Inicio';
    this.pageSub = data['subtitle'] ?? 'Panel de control principal';
  }
}
