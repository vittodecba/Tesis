// Umbral por debajo del cual un insumo o producto se considera "stock bajo".
// (Insumo/ProductoStock no tienen un campo de mínimo propio todavía — mejora futura.)
export const STOCK_BAJO_UMBRAL = 10;

// Estados numéricos del Lote (api/PlanProduccion).
// Coincide con el mapeo usado en lote-listado.html
export const LOTE_ESTADO = {
  CANCELADO: 0,
  PLANIFICADO: 1,
  EN_PROCESO: 2,
  FINALIZADO: 3,
} as const;
