export interface Lote {
  // ── De PlanificacionService (api/PlanProduccion) ──────────────
  id: number;
  loteId: number;
  recetaId: number;
  recetaNombre?: string; // ← nuevo
  estilo?: string; // ← nuevo (viene de Receta)
  fermentadorId: number;
  fermentadorNombre?: string;
  volumenLitros: number;
  fechaInicio?: Date;
  fechaFinEstimada?: Date;
  estado: number;
  observaciones?: string;
  usuarioId?: number;
  fechaCreacion?: Date;

  // ── De LoteService (api/Lote) ─────────────────────────────────
  codigo?: string;
  fechaElaboracion?: Date;
  inoculo?: string;
  responsable?: string;
  diasEstimadosFermentacion?: number;
  fechaFinReal?: Date;
}
