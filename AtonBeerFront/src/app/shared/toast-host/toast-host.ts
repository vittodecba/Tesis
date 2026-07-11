import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule, CheckCircle2, XCircle, Info, AlertTriangle, X } from 'lucide-angular';
import { NotificationService, Toast } from '../../core/services/notification.service';

@Component({
  selector: 'app-toast-host',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="fixed bottom-6 right-6 z-[100] flex flex-col gap-2 pointer-events-none">
      <div *ngFor="let t of noti.toasts()"
           class="pointer-events-auto flex items-center gap-3 rounded-xl shadow-lg px-5 py-3 text-sm font-bold text-white min-w-[260px] max-w-sm animate-[slideIn_0.2s_ease-out]"
           [ngClass]="{
             'bg-emerald-500': t.tipo === 'success',
             'bg-red-500': t.tipo === 'error',
             'bg-[#E67E22]': t.tipo === 'warning',
             'bg-slate-700': t.tipo === 'info'
           }">
        <lucide-icon [img]="icono(t)" class="h-5 w-5 shrink-0"></lucide-icon>
        <span class="flex-1">{{ t.mensaje }}</span>
        <button (click)="noti.cerrar(t.id)" class="shrink-0 opacity-80 hover:opacity-100 transition">
          <lucide-icon [img]="X" class="h-4 w-4"></lucide-icon>
        </button>
      </div>
    </div>
  `,
  styles: [`
    @keyframes slideIn {
      from { opacity: 0; transform: translateX(1rem); }
      to { opacity: 1; transform: translateX(0); }
    }
  `]
})
export class ToastHostComponent {
  readonly noti = inject(NotificationService);
  readonly X = X;

  private iconos = {
    success: CheckCircle2,
    error: XCircle,
    warning: AlertTriangle,
    info: Info
  };

  icono(t: Toast) {
    return this.iconos[t.tipo];
  }
}
