import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  FormsModule,
  Validators,
} from '@angular/forms';
import {
  LucideAngularModule,
  Search,
  Plus,
  X,
  Pencil,
  FileText,
  Calendar,
  Package,
  Hash,
  Mail,
  MapPin,
  Filter,
} from 'lucide-angular';
import { ClientesApiService } from '../../services/clientes-api';

@Component({
  selector: 'app-clientes',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, LucideAngularModule],
  templateUrl: './clientes.component.html',
  styleUrls: ['./clientes.component.scss'],
})
export class ClientesComponent implements OnInit {
  Search = Search;
  Plus = Plus;
  X = X;
  Pencil = Pencil;
  FileText = FileText;
  Calendar = Calendar;
  Package = Package;
  Hash = Hash;
  Mail = Mail;
  MapPin = MapPin;
  Filter = Filter;

  clientes: any[] = [];
  clientesFiltrados: any[] = [];
  cargando = false;

  // Filtros
  filtroBusqueda: string = '';
  filtroTipo: string = '';
  filtroEstado: string = '';

  // Modales
  showCreate = false;
  showSummary = false;
  isEditing = false;
  selectedCliente: any = null;
  selectedId: number | null = null;
  saving = false;
  createError: string | null = null;
  form: FormGroup;

  constructor(
    private api: ClientesApiService,
    private fb: FormBuilder,
  ) {
    this.form = this.fb.group({
      razonSocial: ['', [Validators.required]],
      cuit: ['', [Validators.required]],
      tipoCliente: ['Externo', [Validators.required]],
      email: ['', [Validators.email]],
      ubicacion: ['', [Validators.required]],
      contactoNombre: [''],
      contactoTelefono: [''],
      contactoEmail: ['', [Validators.email]],
    });
  }

  ngOnInit(): void {
    this.loadClientes();
  }

  loadClientes(): void {
    this.cargando = true;
    this.api.getAll().subscribe({
      next: (data) => {
        this.clientes = data ?? [];
        this.aplicarFiltros();
        this.cargando = false;
      },
      error: () => {
        this.cargando = false;
      },
    });
  }

  // --- LÓGICA DE FILTRADO ---
  aplicarFiltros(): void {
    this.clientesFiltrados = this.clientes.filter((c) => {
      const cumpleBusqueda =
        c.razonSocial.toLowerCase().includes(this.filtroBusqueda.toLowerCase()) ||
        c.cuit.includes(this.filtroBusqueda);
      const cumpleTipo = this.filtroTipo === '' || c.tipoCliente === this.filtroTipo;
      const cumpleEstado = this.filtroEstado === '' || c.estadoCliente === this.filtroEstado;

      return cumpleBusqueda && cumpleTipo && cumpleEstado;
    });
  }

  toggleEstado(cliente: any): void {
    const nuevoEstado = cliente.estadoCliente === 'Activo' ? 'Inactivo' : 'Activo';
    this.api.patch(cliente.idCliente, { estadoCliente: nuevoEstado }).subscribe({
      next: () => {
        cliente.estadoCliente = nuevoEstado;
        this.aplicarFiltros(); // Re-filtramos por si el filtro de estado está activo
      },
      error: () => {
        this.loadClientes();
      },
    });
  }

  openCreate(): void {
    this.isEditing = false;
    this.selectedId = null;
    this.form.reset({ tipoCliente: 'Externo' });
    this.form.get('cuit')?.enable();
    this.showCreate = true;
  }

  openEdit(cliente: any): void {
    this.isEditing = true;
    this.selectedId = cliente.idCliente;
    this.form.patchValue(cliente);
    this.form.get('cuit')?.disable();
    this.showCreate = true;
  }

  verResumen(cliente: any): void {
    this.selectedCliente = cliente;
    this.showSummary = true;
  }

  closeModals(): void {
    if (this.saving) return;
    this.showCreate = false;
    this.showSummary = false;
    this.selectedCliente = null;
    this.createError = null;
  }

  submitForm(): void {
    if (this.form.invalid) return;
    this.saving = true;
    const rawValue = this.form.getRawValue();
    const dto = { ...rawValue, cuit: rawValue.cuit.toString().replace(/[^0-9]/g, '') };

    const request =
      this.isEditing && this.selectedId
        ? this.api.update(this.selectedId, dto)
        : this.api.create(dto);

    request.subscribe({
      next: () => {
        this.saving = false;
        this.closeModals();
        this.loadClientes();
      },
      error: (err: any) => {
        this.saving = false;
        this.createError = err.error?.message || 'Error en el servidor';
      },
    });
  }
}
