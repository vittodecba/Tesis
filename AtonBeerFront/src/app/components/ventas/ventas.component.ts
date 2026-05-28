import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule, CreditCard } from 'lucide-angular';
import { VentasService, VentaDto } from '../../services/ventas.service';

@Component({
  selector: 'app-ventas',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './ventas.component.html'
})
export class VentasComponent implements OnInit {
  readonly CreditCard = CreditCard;

  ventas: VentaDto[] = [];
  cargando: boolean = true;
  error: string = '';

  constructor(
    private ventasService: VentasService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.cargarVentas();
  }

  cargarVentas(): void {
    this.cargando = true;
    this.error = '';
    this.ventasService.getVentas().subscribe({
      next: (data) => {
        this.ventas = data;
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'No se pudieron cargar las ventas. Verificá que el servidor esté en línea.';
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  obtenerClaseEstadoVenta(estado: string): string {
    switch (estado) {
      case 'Pendiente':
        return 'bg-amber-100 text-amber-800 border border-amber-200';
      case 'Pagado':
        return 'bg-emerald-100 text-emerald-800 border border-emerald-200';
      default:
        return 'bg-slate-100 text-slate-800 border border-slate-200';
    }
  }

  obtenerClaseMetodoPago(metodo: string): string {
    switch (metodo) {
      case 'Efectivo':
        return 'bg-green-100 text-green-800 border border-green-200';
      case 'Transferencia':
        return 'bg-blue-100 text-blue-800 border border-blue-200';
      default:
        return 'bg-slate-100 text-slate-800 border border-slate-200';
    }
  }
}
