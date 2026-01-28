import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
  ActivatedRoute,
  NavigationEnd,
} from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { filter } from 'rxjs/operators';
import { AuthService } from '../../core/services/auth.service';
import {
  Home,
  Users,
  LogOut,
  UserCog,
  ShieldCheck,
  Lock,
  LayoutDashboard,
  Settings,
  Beer,
  LineChart,
  User,
  Barrel,
  CreditCard,
  ClipboardList,
  BookOpen,
  CalendarDays,
  FlaskConical,
  Boxes,
  Receipt,
} from 'lucide-angular';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet, LucideAngularModule],
  templateUrl: './layout.html',
  styleUrls: ['./layout.scss'],
})
export class LayoutComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);

  // REGISTRO DE ICONOS (Soluciona errores TS2339)
  Home = Home;
  Users = Users;
  LogOut = LogOut;
  UserCog = UserCog;
  ShieldCheck = ShieldCheck;
  Lock = Lock;
  LayoutDashboard = LayoutDashboard;
  Settings = Settings;
  Beer = Beer;
  LineChart = LineChart;
  User = User;
  Barrel = Barrel;
  CreditCard = CreditCard;
  ClipboardList = ClipboardList;
  BookOpen = BookOpen;
  CalendarDays = CalendarDays;
  FlaskConical = FlaskConical;
  Boxes = Boxes;
  Receipt = Receipt;

  currentUser = this.authService.getCurrentUser();
  pageTitle = 'Inicio';
  pageSub = 'Panel de control';

  ngOnInit() {
    this.updateHeader();
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe(() => this.updateHeader());
  }

  // Necesario para el *ngIf de tu HTML
  esAdmin(): boolean {
    return true;
  }

  private updateHeader() {
    let route = this.activatedRoute;
    while (route.firstChild) route = route.firstChild;

    // El "?" es clave para que no de error de "undefined"
    this.pageTitle = route.snapshot?.data?.['title'] || 'Inicio';
    this.pageSub = route.snapshot?.data?.['subtitle'] || 'Gestión';
  }

  cerrarSesion(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
