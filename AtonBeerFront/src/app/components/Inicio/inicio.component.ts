import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import {
  LucideAngularModule,
  Users,
  Box,
  ClipboardList,
  TrendingUp,
  AlertTriangle,
  Beer,
  FlaskConical,
  Truck,
  Wallet,
  CalendarClock,
  BookOpen,
  Factory,
  Package,
  DollarSign,
  Shield,
  FileText,
} from 'lucide-angular';

import { AuthService } from '../../core/services/auth.service';
import { ROLES } from '../../core/constants/roles';
import { VentasService, VentaDto } from '../../services/ventas.service';
import { PedidoService } from '../../core/services/pedido.service';
import { StockService, MovimientoDetalladoDto } from '../../services/stock.service';
import { LoteService } from '../../services/lote';
import { FermentadorService } from '../../services/fermentador';
import { BarrilService, BarrilDto } from '../../services/barril.service';
import { RecetaService } from '../../services/receta';
import { Lote } from '../../Interfaces/lote';
import { Fermentador } from '../../Interfaces/fermentador';
import { LOTE_ESTADO } from './inicio.constants';

type KpiColor = 'blue' | 'orange' | 'green' | 'purple' | 'red' | 'teal';

interface KpiCard {
  label: string;
  value: string | number;
  icon: any;
  color: KpiColor;
  route?: string;
}

interface AlertItem {
  level: 'danger' | 'warning';
  text: string;
  route?: string;
}

interface Shortcut {
  label: string;
  route: string;
  icon: any;
}

interface ListaHoy {
  titulo: string;
  items: { texto: string; sub?: string; route?: string }[];
}

@Component({
  selector: 'app-inicio',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './inicio.component.html',
  styleUrls: ['./inicio.component.scss'],
})
export class InicioComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);
  private ventasSvc = inject(VentasService);
  private pedidoSvc = inject(PedidoService);
  private stockSvc = inject(StockService);
  private loteSvc = inject(LoteService);
  private fermentadorSvc = inject(FermentadorService);
  private barrilSvc = inject(BarrilService);
  private recetaSvc = inject(RecetaService);

  // Iconos
  AlertTriangle = AlertTriangle;

  currentUser = this.authService.getCurrentUser();
  rolNombre = this.currentUser?.rolNombre ?? '';
  fechaActual = new Date();

  loading = true;
  kpis: KpiCard[] = [];
  alerts: AlertItem[] = [];
  shortcuts: Shortcut[] = [];
  lista: ListaHoy | null = null;

  ngOnInit(): void {
    this.cargarDashboard();
  }

  // ─────────────────────────────────────────────────────────────
  // Carga de datos (solo lo que el rol necesita)
  // ─────────────────────────────────────────────────────────────
  private cargarDashboard(): void {
    const r = this.rolNombre;
    const necesita = {
      ventas: [ROLES.ADMIN, ROLES.GERENTE, ROLES.GERENTE_MAYOR].includes(r as any),
      pedidos: [ROLES.ADMIN, ROLES.GERENTE, ROLES.RESP_PEDIDOS].includes(r as any),
      clientes: [ROLES.ADMIN, ROLES.GERENTE, ROLES.RESP_PEDIDOS].includes(r as any),
      barriles: [ROLES.ADMIN, ROLES.RESP_PEDIDOS, ROLES.RESP_PLANTA].includes(r as any),
      lotes: [ROLES.ADMIN, ROLES.RESP_PLANTA].includes(r as any),
      fermentadores: [ROLES.COCINERO].includes(r as any),
      movimientos: [ROLES.RESP_PLANTA].includes(r as any),
      recetas: [ROLES.RESP_PLANTA, ROLES.COCINERO].includes(r as any),
    };

    const reqs: any = {};
    if (necesita.ventas) reqs.ventas = this.ventasSvc.getVentas().pipe(catchError(() => of([] as VentaDto[])));
    if (necesita.pedidos) reqs.pedidos = this.pedidoSvc.getPedidos().pipe(catchError(() => of([] as any[])));
    if (necesita.clientes) reqs.clientes = this.pedidoSvc.getClientes().pipe(catchError(() => of([] as any[])));
    if (necesita.barriles) reqs.barriles = this.barrilSvc.getBarriles().pipe(catchError(() => of([] as BarrilDto[])));
    if (necesita.lotes) reqs.lotes = this.loteSvc.getLotes().pipe(catchError(() => of([] as Lote[])));
    if (necesita.fermentadores) reqs.fermentadores = this.fermentadorSvc.getFermentadores().pipe(catchError(() => of([] as Fermentador[])));
    if (necesita.movimientos) reqs.movimientos = this.stockSvc.getMovimientos().pipe(catchError(() => of([] as MovimientoDetalladoDto[])));
    if (necesita.recetas) reqs.recetas = this.recetaSvc.getAll().pipe(catchError(() => of([] as any[])));

    // forkJoin de un objeto vacío no emite; cubrimos ese caso.
    if (Object.keys(reqs).length === 0) {
      this.loading = false;
      return;
    }

    forkJoin(reqs).subscribe((data: any) => {
      this.construirVista(data);
      this.loading = false;
    });
  }

  // ─────────────────────────────────────────────────────────────
  // Armado de la vista según rol
  // ─────────────────────────────────────────────────────────────
  private construirVista(d: any): void {
    switch (this.rolNombre) {
      case ROLES.RESP_PEDIDOS:
        this.vistaPedidos(d);
        break;
      case ROLES.RESP_PLANTA:
        this.vistaPlanta(d);
        break;
      case ROLES.COCINERO:
        this.vistaCocinero(d);
        break;
      case ROLES.GERENTE_MAYOR:
        this.vistaGerenteMayor(d);
        break;
      case ROLES.GERENTE:
        this.vistaGerente(d);
        break;
      case ROLES.ADMIN:
        this.vistaAdmin(d);
        break;
      default:
        // Rol sin dashboard específico: solo saludo + atajos básicos
        this.shortcuts = [{ label: 'Ir al inicio', route: '/inicio', icon: TrendingUp }];
    }
  }

  // ── Responsable de Pedidos ───────────────────────────────────
  private vistaPedidos(d: any): void {
    const pedidos: any[] = d.pedidos ?? [];
    const clientes: any[] = d.clientes ?? [];
    const barriles: BarrilDto[] = d.barriles ?? [];

    const pendientes = pedidos.filter((p) => this.estadoPedido(p) === 'Pendiente');
    const entregasHoy = pendientes.filter((p) => this.esHoy(p.fechaEntregaProgramada));
    const entregasVencidas = pendientes.filter((p) => this.esVencida(p.fechaEntregaProgramada));
    const barrilesCliente = barriles.filter((b) => b.clienteId != null);

    this.kpis = [
      { label: 'Pedidos pendientes', value: pendientes.length, icon: ClipboardList, color: 'blue', route: '/pedidos/registrar' },
      { label: 'Entregas para hoy', value: entregasHoy.length, icon: Truck, color: 'orange', route: '/pedidos/registrar' },
      { label: 'Clientes activos', value: this.clientesActivos(clientes), icon: Users, color: 'green', route: '/clientes' },
      { label: 'Barriles en cliente', value: barrilesCliente.length, icon: Box, color: 'purple', route: '/barriles' },
    ];

    if (entregasVencidas.length) {
      this.alerts.push({
        level: 'danger',
        text: `${entregasVencidas.length} pedido(s) con fecha de entrega vencida y aún pendientes.`,
        route: '/pedidos/registrar',
      });
    }

    this.shortcuts = [
      { label: 'Nuevo pedido', route: '/pedidos/registrar', icon: ClipboardList },
      { label: 'Barriles', route: '/barriles', icon: Box },
    ];

    const proximas = [...entregasVencidas, ...entregasHoy].slice(0, 6);
    this.lista = {
      titulo: 'Entregas a atender',
      items: proximas.length
        ? proximas.map((p) => ({
            texto: `Pedido #${p.id ?? p.idPedido ?? ''} — ${p.razonSocial ?? p.clienteNombre ?? 'Cliente'}`,
            sub: this.esVencida(p.fechaEntregaProgramada)
              ? `Vencida: ${this.fechaCorta(p.fechaEntregaProgramada)}`
              : `Hoy: ${this.fechaCorta(p.fechaEntregaProgramada)}`,
            route: '/pedidos/registrar',
          }))
        : [],
    };
  }

  // ── Responsable de Planta (gestión) ──────────────────────────
  private vistaPlanta(d: any): void {
    const lotes: Lote[] = d.lotes ?? [];
    const movimientos: MovimientoDetalladoDto[] = d.movimientos ?? [];
    const recetas: any[] = d.recetas ?? [];
    const barriles: BarrilDto[] = d.barriles ?? [];

    const enProceso = lotes.filter((l) => this.estadoLote(l) === LOTE_ESTADO.EN_PROCESO);
    const planificados = lotes.filter((l) => this.estadoLote(l) === LOTE_ESTADO.PLANIFICADO);
    const producidoMes = this.producidoEsteMes(movimientos);

    const listosFinalizar = enProceso.filter((l) => this.esHoyOAntes(l.fechaFinEstimada));

    this.kpis = [
      { label: 'Lotes en proceso', value: enProceso.length, icon: FlaskConical, color: 'blue', route: '/planificacion' },
      { label: 'Lotes planificados', value: planificados.length, icon: CalendarClock, color: 'purple', route: '/planificacion' },
      { label: 'Recetas', value: recetas.length, icon: BookOpen, color: 'teal', route: '/recetas' },
      { label: 'Producido este mes (u)', value: producidoMes, icon: Beer, color: 'orange', route: '/stock' },
      { label: 'Barriles', value: barriles.length, icon: Box, color: 'green', route: '/barriles' },
    ];

    if (listosFinalizar.length) {
      this.alerts.push({
        level: 'warning',
        text: `${listosFinalizar.length} lote(s) llegaron a su fecha estimada de fin — listos para finalizar / designar volumen.`,
        route: '/planificacion',
      });
    }

    this.shortcuts = [
      { label: 'Nuevo lote', route: '/planificacion/nueva', icon: FlaskConical },
      { label: 'Recetas', route: '/recetas', icon: BookOpen },
      { label: 'Stock', route: '/stock', icon: Box },
      { label: 'Barriles', route: '/barriles', icon: Package },
      { label: 'Pedidos', route: '/pedidos/registrar', icon: ClipboardList },
    ];

    this.lista = {
      titulo: 'Lotes en marcha',
      items: enProceso.slice(0, 6).map((l) => ({
        texto: `${l.recetaNombre ?? l.codigo ?? 'Lote'} — ${l.volumenLitros ?? 0} L`,
        sub: l.fechaFinEstimada ? `Fin estimado: ${this.fechaCorta(l.fechaFinEstimada)}` : undefined,
        route: '/planificacion',
      })),
    };
  }

  // ── Cocinero (operativo) ─────────────────────────────────────
  private vistaCocinero(d: any): void {
    const fermentadores: Fermentador[] = d.fermentadores ?? [];
    const recetas: any[] = d.recetas ?? [];

    const ocupados = fermentadores.filter((f) => this.norm(f.estado) === 'ocupado');
    const disponibles = fermentadores.filter((f) => this.norm(f.estado) === 'disponible');

    this.kpis = [
      { label: 'En fermentación', value: ocupados.length, icon: Factory, color: 'orange', route: '/fermentadores' },
      { label: 'Fermentadores libres', value: disponibles.length, icon: Box, color: 'green', route: '/fermentadores' },
      { label: 'Recetas', value: recetas.length, icon: BookOpen, color: 'purple', route: '/recetas' },
    ];

    // Fallback acordado: "registros de fermentación pendientes hoy" se aproxima con
    // los fermentadores ocupados (cada uno requiere su carga diaria de pH/densidad/temp).
    if (ocupados.length) {
      this.alerts.push({
        level: 'warning',
        text: `${ocupados.length} fermentador(es) en fermentación — recordá cargar el registro diario.`,
        route: '/fermentadores',
      });
    }
    this.shortcuts = [
      { label: 'Fermentadores', route: '/fermentadores', icon: Factory },
      { label: 'Recetas', route: '/recetas', icon: BookOpen },
      { label: 'Stock', route: '/stock', icon: Box },
      { label: 'Insumos', route: '/insumos', icon: Package },
    ];

    this.lista = {
      titulo: 'Fermentadores ocupados',
      items: ocupados.slice(0, 6).map((f) => ({
        texto: `${f.nombre} — ${f.estiloNombre ?? f.codigoLote ?? 'Lote en curso'}`,
        sub: f.volumenLitrosLote ? `${f.volumenLitrosLote} L` : undefined,
        route: '/fermentadores',
      })),
    };
  }

  // ── Gerente Mayor (ventas / cobros) ──────────────────────────
  private vistaGerenteMayor(d: any): void {
    const ventas: VentaDto[] = d.ventas ?? [];
    const delMes = this.ventasDelMes(ventas);
    const totalMes = this.suma(delMes, 'montoTotal');
    const cobradoMes = delMes.reduce((acc, v) => acc + (v.totalPagado ?? 0), 0);
    const ticket = delMes.length ? totalMes / delMes.length : 0;

    this.kpis = [
      { label: 'Ventas del mes', value: this.money(totalMes), icon: TrendingUp, color: 'green', route: '/ventas' },
      { label: 'N° de ventas (mes)', value: delMes.length, icon: ClipboardList, color: 'blue', route: '/ventas' },
      { label: 'Cobrado este mes', value: this.money(cobradoMes), icon: Wallet, color: 'teal', route: '/ventas' },
      { label: 'Ticket promedio', value: this.money(ticket), icon: DollarSign, color: 'purple', route: '/ventas' },
    ];

    this.shortcuts = [
      { label: 'Ventas', route: '/ventas', icon: TrendingUp },
      { label: 'Reportes y PDF', route: '/ventas/reporte', icon: FileText },
    ];

    const recientes = [...delMes].sort(
      (a, b) => (this.parse(b.fechaCreacion)?.getTime() ?? 0) - (this.parse(a.fechaCreacion)?.getTime() ?? 0),
    );
    this.lista = {
      titulo: 'Ventas recientes del mes',
      items: recientes.slice(0, 6).map((v) => ({
        texto: `${v.numeroVenta ?? 'Venta'} — ${v.clienteNombre ?? 'Cliente'}`,
        sub: `${this.money(v.montoTotal)} · ${this.fechaCorta(v.fechaCreacion)}`,
        route: '/ventas',
      })),
    };
  }

  // ── Gerente (comercial) ──────────────────────────────────────
  private vistaGerente(d: any): void {
    const ventas: VentaDto[] = d.ventas ?? [];
    const pedidos: any[] = d.pedidos ?? [];
    const clientes: any[] = d.clientes ?? [];

    const pendientesCobro = ventas.filter((v) => this.norm(v.estadoVenta) === 'pendiente');
    const vencidas = pendientesCobro.filter((v) => this.esVencida(v.plazo));
    const pedidosPendientes = pedidos.filter((p) => this.estadoPedido(p) === 'Pendiente');

    this.kpis = [
      { label: 'Cobros vencidos', value: vencidas.length, icon: AlertTriangle, color: 'red', route: '/ventas' },
      { label: 'Pendiente de cobro', value: this.money(this.sumaPendiente(pendientesCobro)), icon: Wallet, color: 'orange', route: '/ventas' },
      { label: 'Pedidos pendientes', value: pedidosPendientes.length, icon: ClipboardList, color: 'blue', route: '/pedidos/registrar' },
      { label: 'Clientes activos', value: this.clientesActivos(clientes), icon: Users, color: 'purple', route: '/clientes' },
    ];

    this.alertasCobro(vencidas);

    this.shortcuts = [
      { label: 'Cobrar / Ventas', route: '/ventas', icon: Wallet },
      { label: 'Roles y accesos', route: '/roles', icon: Shield },
      { label: 'Clientes', route: '/clientes', icon: Users },
      { label: 'Pedidos', route: '/pedidos/registrar', icon: ClipboardList },
    ];

    this.listaCobros(vencidas);
  }

  // ── Administrador (resumen combinado) ────────────────────────
  private vistaAdmin(d: any): void {
    const lotes: Lote[] = d.lotes ?? [];
    const pedidos: any[] = d.pedidos ?? [];
    const ventas: VentaDto[] = d.ventas ?? [];

    const enProceso = lotes.filter((l) => this.estadoLote(l) === LOTE_ESTADO.EN_PROCESO);
    const pedidosPendientes = pedidos.filter((p) => this.estadoPedido(p) === 'Pendiente');
    const vencidas = ventas.filter((v) => this.norm(v.estadoVenta) === 'pendiente' && this.esVencida(v.plazo));
    const totalMes = this.suma(this.ventasDelMes(ventas), 'montoTotal');

    this.kpis = [
      { label: 'Lotes en proceso', value: enProceso.length, icon: FlaskConical, color: 'blue', route: '/planificacion' },
      { label: 'Pedidos pendientes', value: pedidosPendientes.length, icon: ClipboardList, color: 'orange', route: '/pedidos/registrar' },
      { label: 'Cobros vencidos', value: vencidas.length, icon: AlertTriangle, color: 'red', route: '/ventas' },
      { label: 'Ventas del mes', value: this.money(totalMes), icon: TrendingUp, color: 'green', route: '/ventas' },
    ];

    if (vencidas.length) this.alertasCobro(vencidas);

    this.shortcuts = [
      { label: 'Pedidos', route: '/pedidos/registrar', icon: ClipboardList },
      { label: 'Ventas', route: '/ventas', icon: TrendingUp },
      { label: 'Planificación', route: '/planificacion', icon: FlaskConical },
      { label: 'Stock', route: '/stock', icon: Box },
    ];

    this.listaCobros(vencidas, 'Pendientes que requieren atención');
  }

  // ─────────────────────────────────────────────────────────────
  // Helpers de alerta/lista reutilizables (ventas)
  // ─────────────────────────────────────────────────────────────
  private alertasCobro(vencidas: VentaDto[]): void {
    if (vencidas.length) {
      this.alerts.push({
        level: 'danger',
        text: `${vencidas.length} cobro(s) vencido(s) — venta(s) pendientes con plazo cumplido.`,
        route: '/ventas',
      });
    }
  }

  private listaCobros(vencidas: VentaDto[], titulo = 'Cobros vencidos'): void {
    this.lista = {
      titulo,
      items: vencidas.slice(0, 6).map((v) => ({
        texto: `${v.numeroVenta ?? 'Venta'} — ${v.clienteNombre ?? 'Cliente'}`,
        sub: `${this.money(v.saldoPendiente ?? v.montoTotal)} · venció ${this.fechaCorta(v.plazo)}`,
        route: '/ventas',
      })),
    };
  }

  // ─────────────────────────────────────────────────────────────
  // Cálculos / utilidades
  // ─────────────────────────────────────────────────────────────
  private ventasDelMes(ventas: VentaDto[]): VentaDto[] {
    const hoy = new Date();
    return ventas.filter((v) => {
      const f = this.parse(v.fechaCreacion);
      return !!f && f.getMonth() === hoy.getMonth() && f.getFullYear() === hoy.getFullYear();
    });
  }

  // Unidades ingresadas por producción (MovimientoStock motivo "Produccion") en el mes actual.
  private producidoEsteMes(movimientos: MovimientoDetalladoDto[]): number {
    const hoy = new Date();
    return movimientos
      .filter((m) => this.norm(m.motivoMovimiento) === 'produccion')
      .filter((m) => {
        const f = this.parse(m.fecha);
        return !!f && f.getMonth() === hoy.getMonth() && f.getFullYear() === hoy.getFullYear();
      })
      .reduce((acc, m) => acc + (m.cantidad ?? 0), 0);
  }

  private suma(ventas: VentaDto[], campo: 'montoTotal'): number {
    return ventas.reduce((acc, v) => acc + (v[campo] ?? 0), 0);
  }

  private sumaPendiente(ventas: VentaDto[]): number {
    return ventas.reduce((acc, v) => acc + (v.saldoPendiente ?? v.montoTotal ?? 0), 0);
  }

  private clientesActivos(clientes: any[]): number {
    return clientes.filter((c) => (c.estadoCliente ?? c.EstadoCliente) === 'Activo').length;
  }

  private estadoPedido(p: any): string {
    return p.estadoPedido ?? p.estadoNombre ?? '';
  }

  private estadoLote(l: Lote): number {
    if (typeof l.estado === 'number') return l.estado;
    const mapa: Record<string, number> = {
      cancelado: LOTE_ESTADO.CANCELADO,
      descartado: LOTE_ESTADO.CANCELADO,
      planificado: LOTE_ESTADO.PLANIFICADO,
      enproceso: LOTE_ESTADO.EN_PROCESO,
      'en proceso': LOTE_ESTADO.EN_PROCESO,
      finalizado: LOTE_ESTADO.FINALIZADO,
    };
    return mapa[this.norm(l.estado)] ?? -1;
  }

  private norm(v: any): string {
    return (v ?? '').toString().trim().toLowerCase();
  }

  private parse(d: any): Date | null {
    if (!d) return null;
    const date = new Date(d);
    return isNaN(date.getTime()) ? null : date;
  }

  private soloDia(d: Date): number {
    return new Date(d.getFullYear(), d.getMonth(), d.getDate()).getTime();
  }

  private esHoy(d: any): boolean {
    const f = this.parse(d);
    return !!f && this.soloDia(f) === this.soloDia(new Date());
  }

  private esVencida(d: any): boolean {
    const f = this.parse(d);
    return !!f && this.soloDia(f) < this.soloDia(new Date());
  }

  private esHoyOAntes(d: any): boolean {
    const f = this.parse(d);
    return !!f && this.soloDia(f) <= this.soloDia(new Date());
  }

  fechaCorta(d: any): string {
    const f = this.parse(d);
    return f ? f.toLocaleDateString('es-AR') : '-';
  }

  private money(n: number): string {
    return '$' + Math.round(n ?? 0).toLocaleString('es-AR');
  }

  // ─────────────────────────────────────────────────────────────
  // Navegación
  // ─────────────────────────────────────────────────────────────
  irA(route?: string): void {
    if (route) this.router.navigate([route]);
  }
}
