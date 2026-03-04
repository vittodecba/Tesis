export interface HistorialItem {
  id: number;
  fecha: Date; 
  email: string;
  exitoso: boolean;
  detalles: string;
  usuario: string;
}

export interface HistorialFiltros {
  email?: string;
  fecha?: string;
  exito?: boolean | null; 
}