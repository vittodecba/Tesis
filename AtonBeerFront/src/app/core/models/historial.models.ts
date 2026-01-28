export interface HistorialItem {
  id: number;
  fechaIntento: Date; 
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