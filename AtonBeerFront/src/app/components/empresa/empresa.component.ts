import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LucideAngularModule, Building2, Save } from 'lucide-angular';
import { EmpresaService } from '../../services/empresa.service';

@Component({
  selector: 'app-empresa',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
  templateUrl: './empresa.component.html'
})
export class EmpresaComponent implements OnInit {
  readonly Building2 = Building2;
  readonly Save = Save;

  form: FormGroup;
  cargando = true;
  guardando = false;
  toast = { show: false, message: '', type: 'success' as 'success' | 'error' };

  constructor(
    private empresaService: EmpresaService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.form = this.fb.group({
      razonSocial: ['', [Validators.required]],
      cuit: ['', [Validators.required]],
      domicilioComercial: ['', [Validators.required]],
      inicioActividades: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.empresaService.get().subscribe({
      next: (e) => {
        this.form.patchValue({
          razonSocial: e.razonSocial,
          cuit: e.cuit,
          domicilioComercial: e.domicilioComercial,
          inicioActividades: e.inicioActividades?.substring(0, 10)
        });
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.cargando = false;
        this.mostrarToast('No se pudieron cargar los datos de la empresa.', 'error');
      }
    });
  }

  guardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.guardando = true;
    this.empresaService.update(this.form.getRawValue()).subscribe({
      next: () => {
        this.guardando = false;
        this.mostrarToast('Datos de la empresa actualizados.', 'success');
      },
      error: (err) => {
        this.guardando = false;
        const msg = err.error?.mensaje || 'No se pudieron guardar los cambios.';
        this.mostrarToast(msg, 'error');
      }
    });
  }

  private mostrarToast(message: string, type: 'success' | 'error'): void {
    this.toast = { show: true, message, type };
    this.cdr.detectChanges();
    setTimeout(() => {
      this.toast.show = false;
      this.cdr.detectChanges();
    }, 3000);
  }
}
