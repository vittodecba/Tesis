import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { VentaDto, VentasService } from '../../services/ventas.service';

@Component({
  selector: 'app-ventas-listado',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './ventas-listado.html'
})
export class VentasListadoComponent implements OnInit {
  filtroForm: FormGroup;
  ventas: VentaDto[] = [];
  ventasFiltradas: VentaDto[] = [];
  ventasPaginadas: VentaDto[] = [];

  paginaActual: number = 1;
  itemsPorPagina: number = 6;
  totalPaginas: number = 1;

  showModalEditar: boolean = false;
  ventaEnEdicion: VentaDto | null = null;
  editEstado: string = '';
  editPlazo: string = '';
  editMetodoPago: string = '';
  guardando: boolean = false;

  toast = { show: false, message: '', type: 'success' as 'success' | 'error' };

  constructor(
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef,
    private ventasService: VentasService
  ) {
    this.filtroForm = this.fb.group({
      fecha: [''],
      clienteNombre: [''],
      estadoPago: [''],
      metodoPago: ['']
    });
  }

  ngOnInit(): void {
    this.cargarVentasReales();
    
    this.filtroForm.valueChanges.subscribe(() => {
      this.aplicarFiltros();
    });
  }

  cargarVentasReales(): void {
    this.ventasService.getVentas().subscribe({
      next: (data: VentaDto[]) => {
        this.ventas = data;
        this.aplicarFiltros();
      },
      error: (err: any) => console.error('Error cargando ventas:', err)
    });
  }

  aplicarFiltros(): void {
    const filtros = this.filtroForm.value;
    
    this.ventasFiltradas = this.ventas.filter(v => {
      let cumpleFecha = true;
      let cumpleCliente = true;
      let cumpleEstado = true;
      let cumpleMetodo = true;

      if (filtros.fecha && v.fechaCreacion) {
        cumpleFecha = v.fechaCreacion.startsWith(filtros.fecha);
      }
      
      if (filtros.clienteNombre && v.clienteNombre) {
        cumpleCliente = v.clienteNombre.toLowerCase().includes(filtros.clienteNombre.toLowerCase());
      }

      if (filtros.estadoPago) {
        cumpleEstado = v.estadoVenta === filtros.estadoPago;
      }

      if (filtros.metodoPago) {
        cumpleMetodo = v.metodoPago === filtros.metodoPago;
      }

      return cumpleFecha && cumpleCliente && cumpleEstado && cumpleMetodo;
    });

    this.ventasFiltradas.sort((a, b) => {
      const fechaA = new Date(a.fechaCreacion || 0).getTime();
      const fechaB = new Date(b.fechaCreacion || 0).getTime();
      return fechaB - fechaA;
    });
    
    this.paginaActual = 1;
    this.actualizarPaginacion();
  }

  actualizarPaginacion(): void {
    this.totalPaginas = Math.ceil(this.ventasFiltradas.length / this.itemsPorPagina) || 1;
    const inicio = (this.paginaActual - 1) * this.itemsPorPagina;
    const fin = inicio + this.itemsPorPagina;
    this.ventasPaginadas = this.ventasFiltradas.slice(inicio, fin);
    this.cdr.detectChanges();
  }

  cambiarPagina(nuevaPagina: number): void {
    if (nuevaPagina >= 1 && nuevaPagina <= this.totalPaginas) {
      this.paginaActual = nuevaPagina;
      this.actualizarPaginacion();
    }
  }

  mostrarToast(mensaje: string, tipo: 'success' | 'error'): void {
    this.toast = { show: true, message: mensaje, type: tipo };
    this.cdr.detectChanges();
    setTimeout(() => {
      this.toast.show = false;
      this.cdr.detectChanges();
    }, 3000);
  }

  editarVenta(venta: VentaDto): void {
    this.ventaEnEdicion = venta;
    this.editEstado = venta.estadoVenta;
    this.editPlazo = venta.plazo ? venta.plazo.substring(0, 10) : '';
    this.editMetodoPago = venta.metodoPago || 'Efectivo';
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
        this.cargarVentasReales();
      },
      error: (err: any) => {
        const msg = err.error?.mensaje || 'No se pudo actualizar la venta.';
        this.mostrarToast(msg, 'error');
        this.guardando = false;
        this.cdr.detectChanges();
      }
    });
  }
}