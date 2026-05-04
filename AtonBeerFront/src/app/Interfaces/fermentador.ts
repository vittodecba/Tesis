export interface Fermentador {
  id?: number;
  nombre: string;
  capacidad: number;
  estado: string;
  observaciones: string;
  loteId?: number;
  estiloNombre?: string;
  codigoLote?: string;
  volumenLitrosLote?: number;
  estadoLote?: string;
}
