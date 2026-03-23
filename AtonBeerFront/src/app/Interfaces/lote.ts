export interface Lote {
  id: number;
  loteId: number;
  recetaId: number;
  fermentadorId: number;
  fermentadorNombre?: string;
  volumenLitros: number;
  fechaInicio?: Date;
  fechaFinEstimada?: Date;
  estado: number; // ← solo number, no string
  observaciones?: string;
  usuarioId?: number;

  // De Feature (FermentadorDetalle)
  codigo?: string;
  recetaNombre?: string;
  fechaElaboracion?: Date;
  estilo?: string;
  inoculo?: string;
  responsable?: string;
  diasEstimadosFermentacion?: number;
  fechaFinReal?: Date;
}
