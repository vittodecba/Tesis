export interface HistorialItem {
  id: number;
  fechaIntento: string;
  emailIntentado: string;
  exitoso: boolean;
  detalles: string;
  usuarioId?: string;
}

export interface HistorialFiltros {
  email?: string;
  fecha?: string;
  exito?: boolean | null;
}