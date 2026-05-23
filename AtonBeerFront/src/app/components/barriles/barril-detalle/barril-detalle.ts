import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; 
import { ActivatedRoute } from '@angular/router';
import { BarrilService } from '../../../services/barril.service';
import { ClientesApiService } from '../../../services/clientes-api';
import { LucideAngularModule, Pencil, Check, X } from 'lucide-angular';

@Component({
  selector: 'app-barril-detalle',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './barril-detalle.html',
})
export class BarrilDetalleComponent implements OnInit {
  readonly Pencil = Pencil;
  readonly Check = Check;
  readonly X = X;

  barril: any = null;
  loading = true;
  editandoObs = false;
  tempObs = '';
  guardando = false;

  mostrarModalMovimiento = false;
  guardandoMovimiento = false;
  nuevoMovimiento: any = {
    tipoMovimiento: 1,
    clienteId: null,
    observaciones: ''
  };

  clientes: any[] = [];
  clientesFiltrados: any[] = [];
  busquedaCliente = '';
  clienteSeleccionado: any = null;
  opcionesMovimiento: { valor: number, texto: string }[] = [];

  constructor(
    private route: ActivatedRoute, 
    private _service: BarrilService,
    private clientesApi: ClientesApiService
  ) {}

  ngOnInit(): void {
    this.cargarBarril();
    this.cargarClientes();
  }

  cargarBarril() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this._service.getBarrilById(Number(id)).subscribe({
        next: (data: any) => {
          this.barril = data;
          this.tempObs = data.observaciones;
          this.loading = false;
        },
        error: () => this.loading = false
      });
    }
  }

  cargarClientes() {
    this.clientesApi.getAll().subscribe({
      next: (data) => {
        this.clientes = data ?? [];
        this.clientesFiltrados = this.clientes;
      }
    });
  }

  activarEdicion() {
    this.tempObs = this.barril.observaciones;
    this.editandoObs = true;
  }

  cancelarEdicion() {
    this.editandoObs = false;
  }

  guardarCambios() {
    this.guardando = true;
    const body = { ...this.barril, observaciones: this.tempObs };

    this._service.updateObservaciones(body).subscribe({
      next: () => {
        this.barril.observaciones = this.tempObs;
        this.editandoObs = false;
        this.guardando = false;
      },
      error: (err) => {
        console.error('Error detallado:', err);
        this.guardando = false;
      }
    });
  }

  volver() { window.history.back(); }
///Modificado 2 veces//
 abrirModalMovimiento() {
    console.log('Barril completo:', this.barril);     
    const estadoActual = this.barril.estado || this.barril.estadoTexto || this.barril.estadoNombre || '';
    console.log('Estado detectado por Angular:', estadoActual);  
    this.opcionesMovimiento = this.obtenerOpcionesPorEstado(estadoActual);      
    const primerValorValido = this.opcionesMovimiento.length > 0 ? this.opcionesMovimiento[0].valor : 1;
    this.nuevoMovimiento = { tipoMovimiento: primerValorValido, clienteId: null, observaciones: '' };
    this.limpiarCliente();
    this.mostrarModalMovimiento = true;
  }
  obtenerOpcionesPorEstado(estadoTexto: string): { valor: number, texto: string }[] {
    const estado = (estadoTexto || '').toLowerCase().trim();
    let opciones: { valor: number, texto: string }[] = [];
    const opDespacho = { valor: 1, texto: 'Despacho a Cliente' };
    const opDevolucion = { valor: 2, texto: 'Devolución de Cliente' };
    const opLavadero = { valor: 3, texto: 'Ingreso a Lavadero' };
    const opFinLavado = { valor: 4, texto: 'Fin de Lavado' };
    const opMantenimiento = { valor: 5, texto: 'Envío a Mantenimiento' };
    const opRetornoMant = { valor: 6, texto: 'Retorno de Mantenimiento' };

    console.log(`🤖 Analizando estado: "${estado}"`);

    if (estado === 'lleno' || estado === 'disponible') {
      console.log('✅ Entró en: Lleno/Disponible');
      opciones.push(opDespacho, opLavadero, opMantenimiento);
    } 
    else if (estado === 'con cliente' || estado === 'en cliente') {
      console.log('✅ Entró en: Con Cliente');
      opciones.push(opDevolucion);
    } 
    else if (estado === 'sucio') {
      console.log('✅ Entró en: Sucio');
      opciones.push(opLavadero, opMantenimiento);
    } 
    else if (estado === 'en lavadero' || estado === 'lavando') {
      console.log('✅ Entró en: Lavadero');
      opciones.push(opFinLavado);
    } 
    else if (estado === 'en mantenimiento' || estado === 'mantenimiento') {
      console.log('✅ Entró en: Mantenimiento');
      opciones.push(opRetornoMant);
    } 
    else {
      console.log('⚠️ ESTADO DESCONOCIDO - Mostrando todo por defecto');
      opciones.push(opDespacho, opDevolucion, opLavadero, opFinLavado, opMantenimiento, opRetornoMant);
    }

    return opciones;
  }

  cerrarModalMovimiento() {
    this.mostrarModalMovimiento = false;
  }

  filtrarClientes() {
    const texto = this.busquedaCliente.toLowerCase();
    this.clientesFiltrados = this.clientes.filter(c => 
      c.razonSocial.toLowerCase().includes(texto) || 
      c.cuit.includes(texto)
    );
  }

  seleccionarCliente(cliente: any) {
    this.clienteSeleccionado = cliente;
    this.nuevoMovimiento.clienteId = cliente.idCliente;
    this.busquedaCliente = '';
    this.clientesFiltrados = [];
  }

  limpiarCliente() {
    this.clienteSeleccionado = null;
    this.nuevoMovimiento.clienteId = null;
    this.busquedaCliente = '';
    this.clientesFiltrados = this.clientes;
  }

  formatearMotivo(motivo: string): string {
    const diccionario: any = {
      'DespachoCliente': 'Despacho a Cliente',
      'DevolucionCliente': 'Devolución de Cliente',
      'IngresoLavadero': 'Ingreso a Lavadero',
      'FinLavado': 'Fin de Lavado',
      'EnvioMantenimiento': 'Envío a Mantenimiento',
      'RetornoMantenimiento': 'Retorno de Mantenimiento'
    };
    return diccionario[motivo] || motivo;
  }

  confirmarMovimiento() {
    this.guardandoMovimiento = true;
    
    const dto = {
      barrilId: this.barril.id,
      tipoMovimiento: Number(this.nuevoMovimiento.tipoMovimiento),
      clienteId: this.nuevoMovimiento.clienteId ? Number(this.nuevoMovimiento.clienteId) : undefined,
      clienteNombre: this.clienteSeleccionado ? this.clienteSeleccionado.razonSocial : undefined,
      observaciones: this.nuevoMovimiento.observaciones
    };

    this._service.registrarMovimiento(dto).subscribe({
      next: () => {
        this.guardandoMovimiento = false;
        this.cerrarModalMovimiento();
        this.loading = true;
        this.cargarBarril();
      },
      error: (err) => {
        console.error('Error al registrar movimiento:', err);
        this.guardandoMovimiento = false;
      }
    });
  }

  deshacerUltimoMovimiento() {
    if (confirm('¿Estás seguro de que querés eliminar el último movimiento? El barril volverá a su estado anterior.')) {
      this.loading = true;
      this._service.eliminarUltimoMovimiento(this.barril.id).subscribe({
        next: () => {
          this.cargarBarril();
        },
        error: (err) => {
          console.error('Error al deshacer movimiento:', err);
          this.loading = false;
        }
      });
    }
  }
}