import { Injectable, signal } from '@angular/core';
import Swal from 'sweetalert2';

export interface Toast {
  id: number;
  tipo: 'success' | 'error' | 'info' | 'warning';
  mensaje: string;
}

export interface ConfirmOpts {
  titulo: string;
  texto?: string;
  confirmText?: string;
  cancelText?: string;
  peligro?: boolean;
}

export interface PromptOpts {
  titulo: string;
  texto?: string;
  inputValue?: string;
  placeholder?: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  // Lista reactiva de toasts que renderiza el ToastHostComponent
  readonly toasts = signal<Toast[]>([]);
  private idSeq = 0;

  // Swal centralizado con los colores de la marca AtonBeer
  private swal = Swal.mixin({
    confirmButtonColor: '#E67E22',
    cancelButtonColor: '#9CA3AF',
    buttonsStyling: true
  });

  // --- TOASTS ---
  success(mensaje: string) { this.push('success', mensaje); }
  error(mensaje: string) { this.push('error', mensaje); }
  info(mensaje: string) { this.push('info', mensaje); }
  warning(mensaje: string) { this.push('warning', mensaje); }

  cerrar(id: number) {
    this.toasts.update(list => list.filter(t => t.id !== id));
  }

  private push(tipo: Toast['tipo'], mensaje: string) {
    const id = ++this.idSeq;
    this.toasts.update(list => [...list, { id, tipo, mensaje }]);
    setTimeout(() => this.cerrar(id), 3500);
  }

  // --- CONFIRMACIÓN (reemplaza confirm()) ---
  async confirm(opts: ConfirmOpts): Promise<boolean> {
    const result = await this.swal.fire({
      title: opts.titulo,
      text: opts.texto,
      icon: opts.peligro ? 'warning' : 'question',
      showCancelButton: true,
      confirmButtonText: opts.confirmText ?? (opts.peligro ? 'Eliminar' : 'Aceptar'),
      cancelButtonText: opts.cancelText ?? 'Cancelar',
      confirmButtonColor: opts.peligro ? '#DC2626' : '#E67E22',
      reverseButtons: true
    });
    return result.isConfirmed;
  }

  // --- PROMPT (reemplaza prompt()) ---
  async prompt(opts: PromptOpts): Promise<string | null> {
    const result = await this.swal.fire({
      title: opts.titulo,
      text: opts.texto,
      input: 'text',
      inputValue: opts.inputValue ?? '',
      inputPlaceholder: opts.placeholder ?? '',
      showCancelButton: true,
      confirmButtonText: 'Aceptar',
      cancelButtonText: 'Cancelar',
      reverseButtons: true,
      inputValidator: (value) => (!value || !value.trim() ? 'Este campo es obligatorio' : null)
    });
    return result.isConfirmed ? (result.value as string).trim() : null;
  }
}
