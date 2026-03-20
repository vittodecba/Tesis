// export interface Lote {
//   id: number;
//   numeroLote: string;
//   estilo: string;
//   volumenPlanificado: number;
//   fechaInicioPlanificada: Date;
//   fechaFinEstimada: Date;
//   estado: string;
//   fermentadorId?: number;
//   fermentadorNombre?: string;
//   temperaturaObjetivo?: number;
//   observaciones?: string;
//   recetaId?: number;
//   fechaProduccion?: Date;
// }
export interface Lote {
  id: number;
  loteId: number;
  recetaId: number;
  fermentadorId: number;
  fermentadorNombre?: string;
  volumenLitros: number;
  fechaInicio: Date;
  fechaFinEstimada: Date;
  estado: number;
  observaciones?: string;
  usuarioId: number;
}