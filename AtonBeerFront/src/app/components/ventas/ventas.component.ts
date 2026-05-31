import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, CreditCard, Pencil } from 'lucide-angular';
import { VentasService, VentaDto } from '../../services/ventas.service';

@Component({
  selector: 'app-ventas',
  standalone: true,
  imports: [CommonModule, LucideAngularModule, FormsModule],
  templateUrl: './ventas.component.html'
})
export class VentasComponent implements OnInit {
  readonly CreditCard = CreditCard;
  readonly Pencil = Pencil;

  ventas: VentaDto[] = [];
  cargando: boolean = true;
  error: string = '';

  // Toast
  toast = { show: false, message: '', type: 'success' as 'success' | 'error' };

  // Modal edición
  showModalEditar: boolean = false;
  ventaEnEdicion: VentaDto | null = null;
  editEstado: string = '';
  editPlazo: string = '';
  editMetodoPago: string = '';
  guardando: boolean = false;

  get estaBloqueda(): boolean {
    return this.ventaEnEdicion?.estadoVenta === 'Pagado';
  }

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

  mostrarToast(mensaje: string, tipo: 'success' | 'error'): void {
    this.toast = { show: true, message: mensaje, type: tipo };
    this.cdr.detectChanges();
    setTimeout(() => {
      this.toast.show = false;
      this.cdr.detectChanges();
    }, 3000);
  }

  abrirModalEditar(venta: VentaDto): void {
    this.ventaEnEdicion = venta;
    this.editEstado = venta.estadoVenta;
    this.editPlazo = venta.plazo.substring(0, 10);
    this.editMetodoPago = venta.metodoPago;
    this.guardando = false;
    this.showModalEditar = true;
    this.cdr.detectChanges();
  }

  cerrarModalEditar(): void {
    this.showModalEditar = false;
    this.ventaEnEdicion = null;
    this.guardando = false;
  }

  guardarEdicion(): void {
    if (!this.ventaEnEdicion || this.guardando) return;
    this.guardando = true;
    const dto = {
      estadoVenta: this.editEstado,
      plazo: this.editPlazo,
      metodoPago: this.editMetodoPago
    };
    this.ventasService.patchVenta(this.ventaEnEdicion.id, dto).subscribe({
      next: () => {
        this.mostrarToast('Venta actualizada correctamente.', 'success');
        this.cerrarModalEditar();
        this.cargarVentas();
      },
      error: (err) => {
        const msg = err.error?.mensaje || 'No se pudo actualizar la venta.';
        this.mostrarToast(msg, 'error');
        this.guardando = false;
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
