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
import { ROLES } from '../../core/constants/roles';
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
  FlaskConical, // <--- IMPORTADO
  Boxes,
  Receipt,
  Ruler,
  History,
  Building2
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

  // REGISTRO DE ICONOS 
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
  FlaskConical = FlaskConical; // <--- REGISTRADO PARA EL HTML
  Boxes = Boxes;
  Receipt = Receipt;
  Ruler = Ruler;

  currentUser: any;
  pageTitle = 'Inicio';
  pageSub = 'Panel de control';

  //Historial
  History = History;
  Building2 = Building2;

  ngOnInit() {
    this.currentUser = this.authService.getCurrentUser();
    this.updateHeader();
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe(() => {
        this.updateHeader();
        this.currentUser = this.authService.getCurrentUser();
      });
  }

  hasRole(...roles: string[]): boolean {
    return this.authService.hasRole(...roles);
  }

  puedeVerAdministracion(): boolean {
    return this.hasRole(ROLES.ADMIN, ROLES.GERENTE);
  }

  puedeVerProduccion(): boolean {
    return this.hasRole(ROLES.ADMIN, ROLES.RESP_PLANTA, ROLES.COCINERO);
  }

  puedeVerPlanificacion(): boolean {
    return this.hasRole(ROLES.ADMIN, ROLES.RESP_PLANTA);
  }

  puedeVerBarriles(): boolean {
    return this.hasRole(ROLES.ADMIN, ROLES.RESP_PLANTA, ROLES.RESP_PEDIDOS);
  }

  puedeVerClientes(): boolean {
    return this.hasRole(ROLES.ADMIN, ROLES.GERENTE, ROLES.RESP_PEDIDOS);
  }

  puedeVerPedidos(): boolean {
    return this.hasRole(ROLES.ADMIN, ROLES.GERENTE, ROLES.RESP_PLANTA, ROLES.RESP_PEDIDOS);
  }

  puedeVerVentas(): boolean {
    return this.hasRole(ROLES.ADMIN, ROLES.GERENTE, ROLES.GERENTE_MAYOR);
  }

  puedeVerReportes(): boolean {
    return this.hasRole(ROLES.ADMIN, ROLES.GERENTE_MAYOR);
  }

  private updateHeader() {
    let route = this.activatedRoute;
    while (route.firstChild) route = route.firstChild;
    
    this.pageTitle = route.snapshot?.data?.['title'] || 'Inicio';
    this.pageSub = route.snapshot?.data?.['subtitle'] || 'Gestión';
  }

  cerrarSesion(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}