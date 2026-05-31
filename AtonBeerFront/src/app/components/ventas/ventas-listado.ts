import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Venta } from './models/venta.model';

@Component({
  selector: 'app-ventas-listado',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './ventas-listado.html'
})
export class VentasListadoComponent implements OnInit {
  filtroForm: FormGroup;
  ventas: Venta[] = [];
  ventasFiltradas: Venta[] = [];

  constructor(private fb: FormBuilder, private cdr: ChangeDetectorRef) {
    this.filtroForm = this.fb.group({
      fecha: [''],
      clienteNombre: [''],
      estadoPago: ['']
    });
  }

  ngOnInit(): void {
    this.cargarDatosMock();
    
    this.filtroForm.valueChanges.subscribe(() => {
      this.aplicarFiltros();
    });
  }

  cargarDatosMock(): void {
    this.ventas = [
      { idVenta: 1, fechaVenta: '2026-05-25', clienteNombre: 'Bar Centro', total: 85000, estadoPago: 'Pagado', metodoPago: 'Transferencia' },
      { idVenta: 2, fechaVenta: '2026-05-26', clienteNombre: 'Cervecería Sur', total: 140000, estadoPago: 'Pendiente', metodoPago: 'Efectivo' },
      { idVenta: 3, fechaVenta: '2026-05-28', clienteNombre: 'Kiosco Roca', total: 32000, estadoPago: 'Pagado', metodoPago: 'MercadoPago' }
    ];
    this.ventasFiltradas = [...this.ventas];
    this.cdr.detectChanges();
  }

  aplicarFiltros(): void {
    const filtros = this.filtroForm.value;
    
    this.ventasFiltradas = this.ventas.filter(v => {
      let cumpleFecha = true;
      let cumpleCliente = true;
      let cumpleEstado = true;

      if (filtros.fecha) {
        cumpleFecha = v.fechaVenta === filtros.fecha;
      }
      
      if (filtros.clienteNombre) {
        cumpleCliente = v.clienteNombre.toLowerCase().includes(filtros.clienteNombre.toLowerCase());
      }

      if (filtros.estadoPago) {
        cumpleEstado = v.estadoPago === filtros.estadoPago;
      }

      return cumpleFecha && cumpleCliente && cumpleEstado;
    });
    
    this.cdr.detectChanges();
  }
}