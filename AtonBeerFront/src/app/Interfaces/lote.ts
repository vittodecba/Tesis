export interface Lote {
  id: number;
  numeroLote: string;
  estilo: string;
  volumenPlanificado: number;
  fechaInicioPlanificada: Date;
  fechaFinEstimada: Date;
  estado: string;
  fermentadorId?: number;
  fermentadorNombre?: string;
}