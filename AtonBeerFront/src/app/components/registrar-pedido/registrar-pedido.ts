import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PedidoService } from '../../core/services/pedido.service';
import { LucideAngularModule, Receipt, Plus, Trash2 } from 'lucide-angular';

@Component({
  selector: 'app-registrar-pedido',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule, FormsModule],
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
  isEditing = false;
  pedidoIdActual: number | null = null;
  filtroCliente: string = '';
  filtroEstado: string = '';
  criterioOrden: string = 'fecha_desc';
  showViewModal: boolean = false;
  pedidoSeleccionadoVer: any = null;
  menuAccionesAbiertoId: number | null = null;
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

  get pedidosFiltrados(): any[] {
  let resultado = this.pedidos.filter(p => {
    const coincideCliente = !this.filtroCliente || 
      (p.clienteNombre || '').toLowerCase().includes(this.filtroCliente.toLowerCase());
    const coincideEstado = !this.filtroEstado || 
      (p.estadoPedido || p.estadoNombre) === this.filtroEstado;
    return coincideCliente && coincideEstado;
  });

  return resultado.sort((a, b) => {
    switch (this.criterioOrden) {
      case 'fecha_desc':
        return new Date(b.fechaPedido || b.fecha).getTime() - new Date(a.fechaPedido || a.fecha).getTime();
      
      case 'fecha_asc':
        return new Date(a.fechaPedido || a.fecha).getTime() - new Date(b.fechaPedido || b.fecha).getTime();
      
      case 'total_desc':
        return (b.totalPedido || b.total) - (a.totalPedido || a.total);
      
      case 'total_asc':
        return (a.totalPedido || a.total) - (b.totalPedido || b.total);
      
      default:
        return 0;
    }
  });
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
openEdit(pedidoRow: any): void {
    this.isEditing = true;
    this.pedidoIdActual = pedidoRow.idPedido || pedidoRow.id;  
    this.showModal = true;   
    this.pedidoService.getClientes().subscribe(resClientes => {
      this.clientes = resClientes;      
      this.pedidoService.getProductos().subscribe(resProductos => {
        this.productos = resProductos;        
        this.pedidoService.getPedidoPorId(this.pedidoIdActual!).subscribe({
          next: (pedidoCompleto: any) => {
            console.log("🎯 DATOS OFICIALES RECIBIDOS DEL BACKEND:", pedidoCompleto);            
            this.pedidoForm.patchValue({
              clienteId: pedidoCompleto.idCliente ? Number(pedidoCompleto.idCliente) : '',
              observaciones: pedidoCompleto.observaciones || ''
            });             
            while (this.detalles.length !== 0) {
              this.detalles.removeAt(0);
            }     
            const listaDetalles = pedidoCompleto.detalles;
            if (listaDetalles && listaDetalles.length > 0) {
              listaDetalles.forEach((det: any) => {     
                const idProducto = det.productoStockId;
                const cant = det.cantidad || 1;
                const prec = det.precio || 0;             
                const prodMaster = this.productos.find(x => x.id == idProducto);                
                const detalleGroup = this.fb.group({
                  tempFormato: [prodMaster?.formatoEnvaseNombre || '', Validators.required],
                  tempCapacidad: [prodMaster?.capacidadLitros || '', Validators.required],
                  tempEstilo: [prodMaster?.estilo || '', Validators.required], 
                  productoStockId: [idProducto, Validators.required],
                  cantidad: [cant, [Validators.required, Validators.min(1)]],
                  precioUnitario: [prec, [Validators.required, Validators.min(0)]],
                  subtotal: [(cant * prec), [Validators.required, Validators.min(0)]]
                });              
                detalleGroup.get('cantidad')?.valueChanges.subscribe(() => this.recalcularFila(detalleGroup));
                detalleGroup.get('precioUnitario')?.valueChanges.subscribe(() => this.recalcularFila(detalleGroup));
                detalleGroup.get('subtotal')?.valueChanges.subscribe(() => this.recalcularTotalFinal());

                this.detalles.push(detalleGroup);
              });
            } else {            
              this.agregarDetalle();
            }            
            this.recalcularTotalFinal();
            this.cdr.detectChanges();
          },
          error: (err: any) => {
            this.mostrarToast('No se pudo recuperar el pedido desde el servidor.', 'error');
          }
        });
      });
    });
  }

  closeModal(): void {
    this.showModal = false;
  }

  guardarPedido(): void {
    if (this.pedidoForm.invalid) return;

    const v = this.pedidoForm.value;
    const body: any = {
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
    if (this.isEditing && this.pedidoIdActual) {    
      body.id = this.pedidoIdActual; 
      this.pedidoService.actualizarPedido(this.pedidoIdActual, body).subscribe({
        next: () => {
          this.mostrarToast('¡Pedido actualizado con éxito en Aton Beer!', 'success');
          this.cargarDatos();
          this.closeModal();
        },
        error: (err) => {
          const msg = err.error?.mensaje || 'Error al actualizar el pedido.';
          this.mostrarToast('No se pudo actualizar: ' + msg, 'error');
        }
      });
    } else {    
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
  }
 OpenDetail(pedidoRow: any): void {
  const idBuscar = pedidoRow.idPedido || pedidoRow.id;

  this.pedidoService.getPedidoPorId(idBuscar).subscribe({
    next: (pedidoCompleto: any) => {
      console.log('Detalle recibido:', pedidoCompleto);

      this.pedidoSeleccionadoVer = pedidoCompleto;
      this.showViewModal = true;
      this.cdr.detectChanges();
    },
    error: () => {
      this.mostrarToast('No se pudo cargar el detalle.', 'error');
    }
  });
}

  closeViewModal(): void {
    this.showViewModal = false;
    this.pedidoSeleccionadoVer = null;
  }
  calcularTotalVer(): number {
    if (!this.pedidoSeleccionadoVer || !this.pedidoSeleccionadoVer.detalles) return 0;   
    return this.pedidoSeleccionadoVer.detalles.reduce((sum: number, item: any) => {
      return sum + ((item.cantidad || 0) * (item.precio || 0));
    }, 0);
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


  //NUEVO//
  getPedidoId(pedido: any): number {
  return pedido.idPedido || pedido.id;
}

esPendiente(pedido: any): boolean {
  return (pedido.estadoPedido || pedido.estadoNombre) === 'Pendiente';
}

toggleMenuAcciones(pedido: any): void {
  const id = this.getPedidoId(pedido);
  this.menuAccionesAbiertoId = this.menuAccionesAbiertoId === id ? null : id;
}

cerrarMenuAcciones(): void {
  this.menuAccionesAbiertoId = null;
}

editarDesdeMenu(pedido: any): void {
  this.cerrarMenuAcciones();
  this.openEdit(pedido);
}

cancelarPedido(pedido: any): void {
  const id = this.getPedidoId(pedido);

  if (!confirm(
  `¿Cancelar el pedido #${id}?\n\nEsta acción cambia el estado del pedido y no se puede deshacer desde esta pantalla.`
)) return;

  this.pedidoService.cancelarPedido(id).subscribe({
    next: () => {
      this.mostrarToast('Pedido cancelado correctamente.', 'success');
      this.cargarDatos();
      this.cerrarMenuAcciones();
    },
    error: (err) => {
      const msg = err.error?.mensaje || 'No se pudo cancelar el pedido.';
      this.mostrarToast(msg, 'error');
    }
  });
}

entregarPedido(pedido: any): void {
  const id = this.getPedidoId(pedido);

  if (!confirm(
  `¿Marcar como entregado el pedido #${id}?\n\nEsta acción cambia el estado del pedido y no se puede deshacer desde esta pantalla.`
)) return;

  this.pedidoService.entregarPedido(id).subscribe({
    next: () => {
      this.mostrarToast('Pedido marcado como entregado.', 'success');
      this.cargarDatos();
      this.cerrarMenuAcciones();
    },
    error: (err) => {
      const msg = err.error?.mensaje || 'No se pudo entregar el pedido.';
      this.mostrarToast(msg, 'error');
    }
  });
}
}