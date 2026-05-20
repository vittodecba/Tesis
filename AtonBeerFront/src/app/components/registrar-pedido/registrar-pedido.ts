import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PedidoService } from '../../core/services/pedido.service';
import { LucideAngularModule, Receipt, Plus, Trash2 } from 'lucide-angular';

@Component({
  selector: 'app-registrar-pedido',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
  templateUrl: './registrar-pedido.html'
})
export class RegistrarPedidoComponent implements OnInit {
  readonly Receipt = Receipt;
  readonly Plus = Plus;
  readonly Trash2 = Trash2;

  toast = { show: false, message: '', type: 'success' as 'success' | 'error' };

  showModal = false;
  pedidoForm: FormGroup;
  clientes: any[] = [];
  productos: any[] = [];
  pedidos: any[] = [];
  fechaMinima: string = '';

  constructor(
    private fb: FormBuilder, 
    private pedidoService: PedidoService, 
    private cdr: ChangeDetectorRef
  ) {
    this.pedidoForm = this.fb.group({
      clienteId: ['', Validators.required],
      fechaEntregaProgramada: ['', Validators.required],
      observaciones: [''],
      totalPedido: [0, [Validators.required, Validators.min(0)]],
      detalles: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.cargarDatos();
    
    const hoy = new Date();
    const manana = new Date(hoy);
    manana.setDate(hoy.getDate() + 1);
    this.fechaMinima = manana.toISOString().split('T')[0];
  }

  prevenirNegativos(event: KeyboardEvent) {
    if (event.key === '-' || event.key === 'e') {
      event.preventDefault();
    }
  }

  mostrarToast(mensaje: string, tipo: 'success' | 'error') {
    this.toast = { show: true, message: mensaje, type: tipo };
    this.cdr.detectChanges();
    setTimeout(() => {
      this.toast.show = false;
      this.cdr.detectChanges();
    }, 3000);
  }

  cargarDatos(): void {
    this.pedidoService.getClientes().subscribe(res => {
      this.clientes = res;
      this.cdr.detectChanges();
    });
    this.pedidoService.getProductos().subscribe(res => {
      this.productos = res;
      this.cdr.detectChanges();
    });
    this.pedidoService.getPedidos().subscribe(res => {
      this.pedidos = res;
      this.cdr.detectChanges();
    });
  }

  get detalles(): FormArray {
    return this.pedidoForm.get('detalles') as FormArray;
  }

  agregarDetalle(): void {
    const detalle = this.fb.group({
      tempFormato: ['', Validators.required],
      tempCapacidad: ['', Validators.required],
      tempEstilo: ['', Validators.required], 
      productoStockId: ['', Validators.required],
      cantidad: [1, [Validators.required, Validators.min(1)]],
      precioUnitario: [0, [Validators.required, Validators.min(0)]],
      subtotal: [0, [Validators.required, Validators.min(0)]]
    });

    detalle.get('cantidad')?.valueChanges.subscribe(() => this.recalcularFila(detalle));
    detalle.get('precioUnitario')?.valueChanges.subscribe(() => this.recalcularFila(detalle));
    detalle.get('subtotal')?.valueChanges.subscribe(() => this.recalcularTotalFinal());

    this.detalles.push(detalle);
    this.recalcularTotalFinal();
  }

  recalcularFila(detalle: FormGroup): void {
    const cant = detalle.get('cantidad')?.value || 0;
    const precio = detalle.get('precioUnitario')?.value || 0;
    detalle.get('subtotal')?.setValue(cant * precio, { emitEvent: true });
  }

  recalcularTotalFinal(): void {
    const total = this.detalles.controls.reduce((acc, c) => {
      return acc + (Number(c.get('subtotal')?.value) || 0);
    }, 0);

    const clienteId = this.pedidoForm.get('clienteId')?.value;
    const cliente = this.clientes.find(c => c.idCliente == clienteId);
    
    let totalFinal = total;
    if (cliente && (cliente.tipoCliente === 'Franquicia' || cliente.esFranquicia)) {
      totalFinal = total * 0.90;
    }

    this.pedidoForm.get('totalPedido')?.setValue(totalFinal, { emitEvent: false });
    this.cdr.detectChanges();
  }

  eliminarDetalle(index: number): void {
    if (this.detalles.length > 1) { 
      this.detalles.removeAt(index); 
      this.recalcularTotalFinal(); 
    }
  }

  getFormatosUnicos() {
    return [...new Set(this.productos.map(p => p.formatoEnvaseNombre))];
  }

  getCapacidadesPorFormato(nombreFormato: string) {
    const filtrados = this.productos.filter(p => p.formatoEnvaseNombre === nombreFormato);
    return [...new Set(filtrados.map(p => p.capacidadLitros))].sort((a, b) => Number(b) - Number(a));
  }

  getEstilosDisponibles(nombreFormato: string, capacidad: number, indexActual: number) {
    const seleccionadosIds = this.detalles.controls
      .filter((_, i) => i !== indexActual)
      .map(c => Number(c.get('productoStockId')?.value));
    
    return this.productos.filter(p => 
      p.formatoEnvaseNombre === nombreFormato && 
      p.capacidadLitros == capacidad &&
      !seleccionadosIds.includes(Number(p.id))
    );
  }

  obtenerProducto(id: any) {
    return this.productos.find(x => x.id == id);
  }

  get totalLitros(): number {
    return this.detalles.controls.reduce((acc, c) => {
      const p = this.obtenerProducto(c.get('productoStockId')?.value);
      return acc + ((c.get('cantidad')?.value || 0) * (p?.capacidadLitros || 0));
    }, 0);
  }

  openCreate(): void {
    this.cargarDatos();
    this.pedidoForm.reset({ totalPedido: 0, clienteId: '', fechaEntregaProgramada: '' });
    while (this.detalles.length !== 0) this.detalles.removeAt(0);
    this.agregarDetalle();
    this.showModal = true;
    this.cdr.detectChanges();
  }

  closeModal(): void {
    this.showModal = false;
  }

  guardarPedido(): void {
    if (this.pedidoForm.invalid) return;

    const v = this.pedidoForm.value;
    const body = {
      idCliente: Number(v.clienteId),
      fechaEntregaProgramada: v.fechaEntregaProgramada,
      observaciones: v.observaciones || "",
      totalPedido: Number(v.totalPedido),
      detalles: v.detalles.map((d: any) => ({
        productoStockId: Number(d.productoStockId),
        cantidad: Number(d.cantidad),
        precio: Number(d.precioUnitario)
      }))
    };

    this.pedidoService.crearPedido(body).subscribe({
      next: () => {
        this.mostrarToast('¡Pedido registrado con éxito!', 'success');
        this.cargarDatos();
        this.closeModal();
      },
      error: (err) => {
        const msg = err.error?.errors ? JSON.stringify(err.error.errors) : 'Error en los datos del pedido.';
        this.mostrarToast('No se pudo registrar: ' + msg, 'error');
      }
    });
  }

  getEstilosUnicos(nombreFormato: string, capacidad: number) {
    const filtrados = this.productos.filter(p => 
      p.formatoEnvaseNombre === nombreFormato && 
      p.capacidadLitros == capacidad
    );
    return [...new Set(filtrados.map(p => p.estilo))];
  }

  getRecetasPorEstilo(nombreFormato: string, capacidad: number, estilo: string, indexActual: number) {
    const seleccionadosIds = this.detalles.controls
      .filter((_, i) => i !== indexActual)
      .map(c => Number(c.get('productoStockId')?.value));
    
    return this.productos.filter(p => 
      p.formatoEnvaseNombre === nombreFormato && 
      p.capacidadLitros == capacidad &&
      p.estilo === estilo &&
      !seleccionadosIds.includes(Number(p.id))
    );
  }
}