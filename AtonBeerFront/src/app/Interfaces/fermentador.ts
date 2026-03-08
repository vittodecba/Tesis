export interface Fermentador {
  id?: number;
  nombre: string;
  capacidad: number;
  estado: any; // Usamos any para evitar choques de tipos en el mapeo
  observaciones: string;
}
