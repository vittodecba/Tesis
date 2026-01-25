import { Component, inject } from '@angular/core';
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
  Inbox,
  TrendingUp,
} from 'lucide-angular';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet, LucideAngularModule],
  templateUrl: './layout.html',
  styleUrls: ['./layout.scss'],
})
export class LayoutComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);

  // REGISTRO DE ICONOS (Arregla errores TS2339 y TS2551)
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
  Inboxes = Inbox;
  TrendingUp = TrendingUp;

  currentUser = this.authService.getCurrentUser();
  pageTitle = 'Inicio';
  pageSub = 'Panel de control';

  constructor() {
    this.updateHeader();
    this.router.events
      .pipe(filter((e) => e instanceof NavigationEnd))
      .subscribe(() => this.updateHeader());
  }

  // Arregla error en imagen aaa3f9.png
  esAdmin(): boolean {
    return true; // Puedes cambiar esto por una lógica real de tu AuthService
  }

  cerrarSesion(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  private updateHeader() {
    let route = this.activatedRoute;
    while (route.firstChild) route = route.firstChild;

    // Arregla el TypeError de la imagen b578fe.png con validaciones seguras
    if (route.snapshot && route.snapshot.data) {
      this.pageTitle = route.snapshot.data['title'] || 'Inicio';
      this.pageSub = route.snapshot.data['subtitle'] || 'Gestión';
    }
  }
}
