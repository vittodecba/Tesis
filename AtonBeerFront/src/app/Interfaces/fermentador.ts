export interface Fermentador {
  id?: number;
  nombre: string;
  capacidad: number;
  estado: string; // Mantenlo como string
  observaciones: string;
  loteId?: number;
  estiloNombre?: string;
}
