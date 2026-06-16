import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule } from '@angular/forms';
import {  AplicarDescuentoDto, PagosDto, VentaDto, VentasService } from '../../services/ventas.service';

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
//PAGO//
showModalPago: boolean = false;
ventaParaPago: VentaDto | null = null;
pagoMonto: number | null = null;
pagoFecha: string = '';
pagoMetodoPago: string = 'Efectivo';
registrandoPago: boolean = false;
fechaHoy: string = new Date().toISOString().split('T')[0];

showModalHistorialPagos: boolean = false;
showModalConfirmacion: boolean = false;
ventaHistorialPagos: VentaDto | null = null;
pagosHistorial: PagosDto[] = [];
cargandoHistorialPagos: boolean = false;
errorPago: string = '';


// DESCUENTOS
showModalDescuento: boolean = false;
ventaParaDescuento: VentaDto | null = null;
tipoDescuento: 'Porcentaje' | 'MontoFijo' = 'Porcentaje';
valorDescuento: number | null = null;
motivoDescuento: string = '';
guardandoDescuento: boolean = false;
errorDescuento: string = '';

facturandoId: number | null = null;


  constructor(
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef,
    private ventasService: VentasService
  ) {
    this.filtroForm = this.fb.group({
      fechaDesde: [''],
      fechaHasta: [''],
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

  obtenerEstadoReal(venta: VentaDto): string {
    if (venta.estadoVenta === 'Pendiente' && venta.plazo) {
      const hoy = new Date();
      hoy.setHours(0, 0, 0, 0);
      const fechaPlazo = new Date(venta.plazo);
      fechaPlazo.setHours(0, 0, 0, 0);
      
      if (fechaPlazo < hoy) {
        return 'Atrasado';
      }
    }
    return venta.estadoVenta;
  }

  aplicarFiltros(): void {
    const filtros = this.filtroForm.value;
    
    this.ventasFiltradas = this.ventas.filter(v => {
      let cumpleFecha = true;
      let cumpleCliente = true;
      let cumpleEstado = true;
      let cumpleMetodo = true;

      if (v.fechaCreacion) {
        const fechaVenta = new Date(v.fechaCreacion);
        fechaVenta.setHours(0, 0, 0, 0);

        if (filtros.fechaDesde) {
          const desde = new Date(filtros.fechaDesde);
          desde.setHours(0, 0, 0, 0);
          if (fechaVenta < desde) cumpleFecha = false;
        }
        
        
        if (filtros.fechaHasta) {
          const hasta = new Date(filtros.fechaHasta);
          hasta.setHours(0, 0, 0, 0);
          if (fechaVenta > hasta) cumpleFecha = false;
        }
      } else if (filtros.fechaDesde || filtros.fechaHasta) {
        cumpleFecha = false;
      }
      
      if (filtros.clienteNombre && v.clienteNombre) {
        cumpleCliente = v.clienteNombre.toLowerCase().includes(filtros.clienteNombre.toLowerCase());
      }

      if (filtros.estadoPago) {
        cumpleEstado = this.obtenerEstadoReal(v) === filtros.estadoPago;
      }

      if (filtros.metodoPago) {
  const metodoVisible = v.totalPagado > 0
    ? v.metodoCobroReal
    : v.metodoPago;

  cumpleMetodo = metodoVisible === filtros.metodoPago;
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
abrirModalDescuento(venta: VentaDto): void {
  this.ventaParaDescuento = venta;
  this.tipoDescuento = 'Porcentaje';
  this.valorDescuento = venta.descuentoPorcentaje || null;
  this.motivoDescuento = venta.motivoDescuento || '';
  this.errorDescuento = venta.totalPagado > 0
    ? 'No se puede modificar el descuento porque la venta ya tiene pagos registrados.'
    : '';
  this.guardandoDescuento = false;
  this.showModalDescuento = true;
  this.cdr.detectChanges();
}

cerrarModalDescuento(): void {
  this.showModalDescuento = false;
  this.ventaParaDescuento = null;
  this.valorDescuento = null;
  this.motivoDescuento = '';
  this.errorDescuento = '';
  this.guardandoDescuento = false;
}

calcularDescuentoPreview(): number {
  if (!this.ventaParaDescuento || !this.valorDescuento) return 0;

  const subtotal = this.ventaParaDescuento.subtotal || this.ventaParaDescuento.montoTotal;

  if (this.tipoDescuento === 'Porcentaje') {
    return subtotal * (Number(this.valorDescuento) / 100);
  }

  return Number(this.valorDescuento);
}

calcularTotalConDescuentoPreview(): number {
  if (!this.ventaParaDescuento) return 0;

  const subtotal = this.ventaParaDescuento.subtotal || this.ventaParaDescuento.montoTotal;
  const descuento = this.calcularDescuentoPreview();

  return subtotal - descuento;
}

guardarDescuento(): void {
  if (!this.ventaParaDescuento || this.guardandoDescuento) return;

  const valor = Number(this.valorDescuento || 0);
  const subtotal = this.ventaParaDescuento.subtotal || this.ventaParaDescuento.montoTotal;

  if (this.ventaParaDescuento.totalPagado > 0) {
    this.errorDescuento = 'No se puede modificar el descuento porque la venta ya tiene pagos registrados.';
    return;
  }

  if (valor <= 0) {
    this.errorDescuento = 'El descuento debe ser mayor a 0.';
    return;
  }

  const descuentoPreview = this.calcularDescuentoPreview();
  if (descuentoPreview >= subtotal) {
    this.errorDescuento = 'El descuento no puede igualar o superar el subtotal.';
    return;
  }

  this.errorDescuento = '';
  this.guardandoDescuento = true;

  const dto: AplicarDescuentoDto = {
    tipoDescuento: this.tipoDescuento,
    valor,
    motivo: this.motivoDescuento || 'Descuento comercial'
  };

  this.ventasService.aplicarDescuento(this.ventaParaDescuento.id, dto).subscribe({
    next: () => {
      this.mostrarToast('Descuento aplicado correctamente.', 'success');
      this.cerrarModalDescuento();
      this.cargarVentasReales();
    },
    error: (err: any) => {
      this.errorDescuento = err.error?.mensaje || 'No se pudo aplicar el descuento.';
      this.guardandoDescuento = false;
      this.cdr.detectChanges();
    }
  });
}

  //Metodos-PAGO//
  abrirModalPago(venta: VentaDto): void {
  this.ventaParaPago = venta;
  this.pagoMonto = null;
  this.pagoMetodoPago = venta.metodoPago || 'Efectivo';
  this.pagoFecha = new Date().toISOString().split('T')[0];
  this.registrandoPago = false;
  this.errorPago = '';
  this.showModalPago = true;
  this.cdr.detectChanges();
}

cerrarModalPago(): void {
  this.showModalPago = false;
  this.ventaParaPago = null;
  this.pagoMonto = null;
  this.registrandoPago = false;
  this.errorPago = '';
}

registrarPago(): void {
    if (!this.ventaParaPago || this.registrandoPago) return;

    const monto = Number(this.pagoMonto || 0);

    // 1. Validaciones
    if (monto <= 0) {
      this.errorPago = 'El monto debe ser mayor a 0.';
      this.cdr.detectChanges();
      return;
    }
    if (monto > this.ventaParaPago.saldoPendiente) {
      this.errorPago = 'El monto no puede superar el saldo pendiente.';
      this.cdr.detectChanges();
      return;
    }

    this.errorPago = ''; 
    
    // 2. En vez de usar el confirm() del navegador, abrimos nuestro propio modal
    this.showModalConfirmacion = true;
  }

  cerrarConfirmacion(): void {
    // Si el usuario se arrepiente, solo cerramos el modal chico
    this.showModalConfirmacion = false;
  }

  confirmarYRegistrarPago(): void {
    // Si el usuario dice que sí, cerramos el cartel y disparamos al backend
    this.showModalConfirmacion = false;
    this.registrandoPago = true;
    
    const monto = Number(this.pagoMonto || 0);

    this.ventasService.registrarPago({
      ventaId: this.ventaParaPago!.id,
      monto,
      fecha: this.pagoFecha,
      metodoPago: this.pagoMetodoPago
    }).subscribe({
      next: () => {
        this.mostrarToast('Pago registrado correctamente.', 'success');
        this.cerrarModalPago(); // Esto cierra el modal grande
        this.cargarVentasReales();
      },
      error: (err: any) => {
        const msg = err.error?.mensaje || 'No se pudo registrar el pago.';
        this.errorPago = msg; 
        this.registrandoPago = false;
      }
    });
  }
  // Genera la factura si no existe; si ya existe, descarga el PDF directamente.
  facturar(venta: VentaDto): void {
    if (this.facturandoId) return;

    if (venta.tieneFactura && venta.facturaId) {
      this.descargarPdf(venta.facturaId);
      return;
    }

    // Solo se puede emitir factura cuando la venta está 100% pagada.
    if (this.obtenerEstadoReal(venta) !== 'Pagado') {
      this.mostrarToast('No se puede facturar: la venta todavía no está 100% pagada.', 'error');
      return;
    }

    this.facturandoId = venta.id;
    this.ventasService.generarFactura(venta.id).subscribe({
      next: (factura) => {
        venta.tieneFactura = true;
        venta.facturaId = factura.id;
        this.facturandoId = null;
        this.mostrarToast(`Factura ${factura.tipo} ${factura.numeroComprobante} generada.`, 'success');
        this.descargarPdf(factura.id);
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.facturandoId = null;
        const msg = err.error?.mensaje || 'No se pudo generar la factura.';
        this.mostrarToast(msg, 'error');
        this.cdr.detectChanges();
      }
    });
  }


  abrirHistorialPagos(venta: VentaDto): void {
  this.ventaHistorialPagos = venta;
  this.pagosHistorial = [];
  this.cargandoHistorialPagos = true;
  this.showModalHistorialPagos = true;

  this.ventasService.getPagosPorVenta(venta.id).subscribe({
    next: (pagos: PagosDto[]) => {
      this.pagosHistorial = pagos;
      this.cargandoHistorialPagos = false;
      this.cdr.detectChanges();
    },
    error: () => {
      this.mostrarToast('No se pudo cargar el historial de pagos.', 'error');
      this.cargandoHistorialPagos = false;
      this.cdr.detectChanges();
    }
  });
}

cerrarHistorialPagos(): void {
  this.showModalHistorialPagos = false;
  this.ventaHistorialPagos = null;
  this.pagosHistorial = [];
  this.cargandoHistorialPagos = false;
}
  private descargarPdf(facturaId: number): void {
    this.ventasService.descargarFacturaPdf(facturaId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Factura_${facturaId}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => this.mostrarToast('No se pudo descargar el PDF de la factura.', 'error')
    });
  }
}