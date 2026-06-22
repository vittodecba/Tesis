// Estados numéricos del Lote (api/PlanProduccion).
// Coincide con el mapeo usado en lote-listado.html
export const LOTE_ESTADO = {
  CANCELADO: 0,
  PLANIFICADO: 1,
  EN_PROCESO: 2,
  FINALIZADO: 3,
} as const;
