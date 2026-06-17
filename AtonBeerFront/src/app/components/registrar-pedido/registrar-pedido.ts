import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PedidoService } from '../../core/services/pedido.service';
import { LucideAngularModule, Receipt, Plus, Trash2 } from 'lucide-angular';
import { BarrilService, BarrilDto } from '../../services/barril.service';


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
  filtroPedidoId: string = '';
  filtroEstado: string = '';
  criterioOrden: string = 'fecha_desc';
  showViewModal: boolean = false;
  pedidoSeleccionadoVer: any = null;
  menuAccionesAbiertoId: number | null = null;
  fechaMinima: string = '';
  paginaActual: number = 1;
  itemsPorPagina: number = 10;

  // --- VARIABLES PARA BARRILES ---
  showModalAsignacionBarriles: boolean = false;
  pedidoParaEntregar: any = null;
  barrilesParaSeleccionar: BarrilDto[] = [];
  listaBarrilesSeleccionadosIds: number[] = [];
  cantidadBarrilesRequeridos: number = 0;
  resumenCapacidades: { [key: string]: { requeridos: number, seleccionados: number, litros: number, estilo: string, receta: string } } = {};
  codigosVerificados: Set<string> = new Set<string>();

  // --- VARIABLES PARA DATOS DE VENTA ---
  showModalDatosVenta: boolean = false;
  pedidoIdParaEntregar: number | null = null;
  barrilesPreSeleccionados: number[] = [];
  plazoVenta: string = '';
  metodoPagoSeleccionado: string = '';

  constructor(
    private fb: FormBuilder, 
    private pedidoService: PedidoService,
    private barrilService: BarrilService, 
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
      this.clientes = res.filter((c: any) => c.estadoCliente === 'Activo' || c.EstadoCliente === 'Activo');
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
  const busquedaCliente = (this.filtroCliente || '').toLowerCase().trim();
  const busquedaPedidoId = (this.filtroPedidoId || '').trim();

  let resultado = this.pedidos.filter(p => {
    const idPedido = String(p.idPedido || p.id || '');
    const clienteNombre = (p.clienteNombre || '').toLowerCase();

    const coincideCliente = !busquedaCliente ||
      clienteNombre.includes(busquedaCliente);

    const coincidePedidoId = !busquedaPedidoId ||
      idPedido.includes(busquedaPedidoId);

    const coincideEstado = !this.filtroEstado ||
      (p.estadoPedido || p.estadoNombre) === this.filtroEstado;

    return coincideCliente && coincidePedidoId && coincideEstado;
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

  
  get totalPaginas(): number {
    return Math.ceil(this.pedidosFiltrados.length / this.itemsPorPagina);
  }  
  get pedidosPaginados(): any[] {
    const inicio = (this.paginaActual - 1) * this.itemsPorPagina;
    const fin = inicio + this.itemsPorPagina;
    return this.pedidosFiltrados.slice(inicio, fin);
  }
  cambiarPagina(pagina: number): void {
    if (pagina >= 1 && pagina <= this.totalPaginas) {
      this.paginaActual = pagina;
    }
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
    this.isEditing = false;
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
            
            this.pedidoForm.patchValue({
              clienteId: pedidoCompleto.idCliente ? Number(pedidoCompleto.idCliente) : '',
              observaciones: pedidoCompleto.observaciones || '',
              totalPedido: 0 // Lo dejamos en 0 inicialmente, el recalcular lo va a sobreescribir al final
            }, { emitEvent: false }); 
            
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
    this.isEditing = false;
  }

  guardarPedido(): void {
    if (this.pedidoForm.invalid) return;

    const v = this.pedidoForm.getRawValue();
    const body: any = {
      idCliente: Number(v.clienteId),
      fechaEntregaProgramada: v.fechaEntregaProgramada,
      observaciones: v.observaciones || "",
      totalPedido: Number(v.totalPedido) || 0,     
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
    const pickingGuardado = localStorage.getItem(`atonbeer_picking_${idBuscar}`);
    if (pickingGuardado) {
      this.codigosVerificados = new Set(JSON.parse(pickingGuardado));
    } else {
      this.codigosVerificados = new Set();
    }

    this.pedidoService.getPedidoPorId(idBuscar).subscribe({
      next: (pedidoCompleto: any) => {
        console.log('Detalle recibido:', pedidoCompleto);
        pedidoCompleto.montoFinal = pedidoRow.totalPedido || pedidoRow.total;
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
    if (!this.pedidoSeleccionadoVer) return 0;     
    return this.pedidoSeleccionadoVer.totalPedido || this.pedidoSeleccionadoVer.total || 0;
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

    if (!confirm(`¿Marcar como entregado el pedido #${id}?`)) return;

    this.pedidoService.getPedidoPorId(id).subscribe({
      next: (pedidoCompleto: any) => {        
        this.resumenCapacidades = {};
        this.cantidadBarrilesRequeridos = 0;        
        
        pedidoCompleto.detalles.forEach((d: any) => {
          const nombreCompleto = (d.productoNombre || '');          
          if (nombreCompleto.toLowerCase().includes('barril')) {
            const cant = d.cantidad || 0;
            this.cantidadBarrilesRequeridos += cant;                     
            const match = nombreCompleto.match(/(\d+)\s*l\b/i);
            const cap = match ? parseInt(match[1], 10) : 0;                    
            const partes = nombreCompleto.split('-').map((p: string) => p.trim());
            const recetaExtraida = partes[0] || '';
            const estiloExtraido = partes.length > 1 ? partes[1] : '';            
            const key = `${cap}-${estiloExtraido}-${recetaExtraida}`;

            if (!this.resumenCapacidades[key]) {
              this.resumenCapacidades[key] = { 
                requeridos: 0, 
                seleccionados: 0, 
                litros: cap, 
                estilo: estiloExtraido, 
                receta: recetaExtraida 
              };
            }
            this.resumenCapacidades[key].requeridos += cant;
          }
        });

        if (this.cantidadBarrilesRequeridos > 0) {
          this.pedidoParaEntregar = pedido;
          this.listaBarrilesSeleccionadosIds = [];
          
          this.barrilService.getBarriles().subscribe(barriles => {           
            this.barrilesParaSeleccionar = barriles.filter(b => {
              if (b.estadoTexto?.toLowerCase() !== 'lleno') return false;

              return Object.values(this.resumenCapacidades).some(req => {
                // Pasamos todo a minúsculas y sacamos espacios extra para evitar errores tontos
                const reqEst = req.estilo.toLowerCase().trim();
                const reqRec = req.receta.toLowerCase().trim();
                const bEst = (b.estilo || '').toLowerCase().trim();
                const bRec = (b.receta || '').toLowerCase().trim();

                const coincide = (req.litros === b.capacidadLitros) && (bEst === reqEst) && (bRec === reqRec);
                
                // ESTO TE VA A MOSTRAR EN LA CONSOLA (F12) QUÉ ESTÁ LEYENDO ANGULAR
                if (b.capacidadLitros === req.litros) {
                  console.log(`🔎 Analizando Barril #${b.codigo} | Backend mandó -> Estilo: '${bEst}', Receta: '${bRec}' | El pedido pide -> Estilo: '${reqEst}', Receta: '${reqRec}' | ¿Coincide?: ${coincide}`);
                }

                return coincide;
              });
            });
            
            this.showModalAsignacionBarriles = true;
            this.cerrarMenuAcciones();
            this.cdr.detectChanges();
          });
        } else {
          this.pedidoIdParaEntregar = id;
          this.barrilesPreSeleccionados = [];
          this.plazoVenta = '';
          this.metodoPagoSeleccionado = '';
          this.showModalDatosVenta = true;
          this.cerrarMenuAcciones();
          this.cdr.detectChanges();
        }
      },
      error: () => this.mostrarToast('No se pudo verificar el detalle del pedido.', 'error')
    });
}

deshacerEntrega(pedido: any): void {
  const id = this.getPedidoId(pedido);

  if (!confirm(
    `¿Estás seguro de que querés deshacer la entrega del pedido #${id}?\n\n` +
    `Esto devolverá el stock virtual a los productos y pondrá los barriles físicos asignados nuevamente en estado 'Lleno' en la fábrica.`
  )) return;

  this.pedidoService.deshacerEntregaPedido(id).subscribe({
    next: () => {
      this.mostrarToast(`Entrega del pedido #${id} revertida. El pedido volvió a 'Pendiente'.`, 'success');         
      localStorage.removeItem(`atonbeer_picking_${id}`);      
      this.cargarDatos();
      this.cerrarMenuAcciones();
      this.cdr.detectChanges();
    },
    error: (err) => {
      const msg = err.error?.mensaje || 'No se pudo deshacer la entrega del pedido.';
      this.mostrarToast(msg, 'error');
    }
  });
}

   toggleVerificacion(codigo: string) {
    if (this.codigosVerificados.has(codigo)) {
      this.codigosVerificados.delete(codigo);
    } else {
      this.codigosVerificados.add(codigo);
    }    
    if (this.pedidoSeleccionadoVer) {
      const id = this.pedidoSeleccionadoVer.id || this.pedidoSeleccionadoVer.idPedido;
      localStorage.setItem(`atonbeer_picking_${id}`, JSON.stringify(Array.from(this.codigosVerificados)));
    }
  }
  progresoVerificacion(codigos: string[]): number {
    if (!codigos || codigos.length === 0) return 0;
    const verificados = codigos.filter(c => this.codigosVerificados.has(c)).length;
    return (verificados / codigos.length) * 100;
  }


 toggleBarrilSeleccionado(barril: any, event: any): void {
    const cap = barril.capacidadLitros || 0;
    const est = barril.estilo || '';
    const rec = barril.receta || '';
    const key = `${cap}-${est}-${rec}`;

    if (!this.resumenCapacidades[key]) return; 

    if (event.target.checked) {    
      if (this.resumenCapacidades[key].seleccionados >= this.resumenCapacidades[key].requeridos) {
        event.target.checked = false;
        this.mostrarToast(`Ya seleccionaste todos los barriles de ${cap}L - ${est} necesarios.`, 'error');
        return;
      }

      this.listaBarrilesSeleccionadosIds.push(barril.id);
      this.resumenCapacidades[key].seleccionados++;

    } else {    
      this.listaBarrilesSeleccionadosIds = this.listaBarrilesSeleccionadosIds.filter(bId => bId !== barril.id);
      this.resumenCapacidades[key].seleccionados--;
    }
  }

  autoAsignarBarriles(): void {    
    this.listaBarrilesSeleccionadosIds = [];
    Object.keys(this.resumenCapacidades).forEach(k => this.resumenCapacidades[k].seleccionados = 0);    
    this.barrilesParaSeleccionar.forEach(b => {
      const cb = document.getElementById('barril-' + b.id) as HTMLInputElement;
      if (cb) cb.checked = false;
    });
    
    Object.keys(this.resumenCapacidades).forEach(key => {
      const req = this.resumenCapacidades[key];
      const reqEst = req.estilo.toLowerCase().trim();
      const reqRec = req.receta.toLowerCase().trim();      
      const barrilesCompatibles = this.barrilesParaSeleccionar.filter(b => {
        const bEst = (b.estilo || '').toLowerCase().trim();
        const bRec = (b.receta || '').toLowerCase().trim();
        return b.capacidadLitros === req.litros && bEst === reqEst && bRec === reqRec;
      });
      
      let asignados = 0;
      for (let b of barrilesCompatibles) {
        if (asignados >= req.requeridos) break;
        this.listaBarrilesSeleccionadosIds.push(b.id);
        this.resumenCapacidades[key].seleccionados++;
        asignados++;       
        const cb = document.getElementById('barril-' + b.id) as HTMLInputElement;
        if (cb) cb.checked = true;
      }
    });

    this.mostrarToast('Asignación automática completada según el stock.', 'success');
  }
  get resumenArray() {
    return Object.keys(this.resumenCapacidades).map(key => {
      const item = this.resumenCapacidades[key];
      return {
        key: key,
        capacidad: item.litros,
        estilo: item.estilo,
        receta: item.receta,
        requeridos: item.requeridos,
        seleccionados: item.seleccionados
      };
    });
  }

  get seleccionCompleta(): boolean {
    return Object.values(this.resumenCapacidades).every(r => r.seleccionados === r.requeridos);
  }

  confirmarEntregaConBarriles(): void {
    const id = this.getPedidoId(this.pedidoParaEntregar);
    this.pedidoIdParaEntregar = id;
    this.barrilesPreSeleccionados = [...this.listaBarrilesSeleccionadosIds];
    this.showModalAsignacionBarriles = false;
    this.plazoVenta = '';
    this.metodoPagoSeleccionado = '';
    this.showModalDatosVenta = true;
    this.cdr.detectChanges();
  }

  confirmarDatosVenta(): void {
    if (!this.plazoVenta || !this.metodoPagoSeleccionado) {
      this.mostrarToast('Completá el plazo y el método de pago para continuar.', 'error');
      return;
    }
    const id = this.pedidoIdParaEntregar!;
    const barriles = this.barrilesPreSeleccionados;
    const plazo = this.plazoVenta;
    const metodo = this.metodoPagoSeleccionado;
    this.showModalDatosVenta = false;
    this.ejecutarEntrega(id, barriles, plazo, metodo);
  }

  cancelarModalVenta(): void {
    this.showModalDatosVenta = false;
    this.pedidoIdParaEntregar = null;
    this.barrilesPreSeleccionados = [];
    this.plazoVenta = '';
    this.metodoPagoSeleccionado = '';
  }

  private ejecutarEntrega(pedidoId: number, barrilesIds: number[], plazo: string, metodoPago: string): void {
    this.pedidoService.entregarPedido(pedidoId, barrilesIds, plazo, metodoPago).subscribe({
      next: () => {
        this.mostrarToast('Pedido entregado y Venta generada correctamente.', 'success');
        this.showModalAsignacionBarriles = false;
        this.listaBarrilesSeleccionadosIds = [];
        this.cargarDatos();
        this.cerrarMenuAcciones();
        this.cdr.detectChanges();
      },
      error: (err) => {
        const msg = err.error?.mensaje || 'No se pudo entregar el pedido.';
        this.mostrarToast(msg, 'error');
      }
    });
  }

ObtenerClaseEstado(estado : string)
{
  switch(estado)
  {
    case 'Pendiente':
      return 'bg-amber-100 text-amber-800 border border-amber-200';
      case 'Entregado':
        return 'bg-emerald-100 text-emerald-800 border border-emerald-200';
       case'Facturado':
           return 'bg-blue-100 text-blue-800 border border-blue-200';
       case 'Cancelado':
  return 'bg-slate-100 text-slate-700 border border-slate-300';

case 'Atrasado':
  return 'bg-red-100 text-red-800 font-bold border border-red-300';
       default:
      return 'bg-slate-100 text-slate-800 border-slate-200';
  }
}
}