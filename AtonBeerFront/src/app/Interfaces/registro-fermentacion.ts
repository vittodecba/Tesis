export interface RegistroFermentacion {
  id?: number;
  loteId: number;
  fecha: string;
  diaFermentacion: number;
  ph: number;
  densidad: number;
  temperatura: number;
  presion?: number | null;
  purgas?: string | null;
  extracciones?: string | null;
  agregados?: string | null;
  observaciones?: string | null;
}
