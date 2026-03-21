export interface Lote {
  id: number;
  codigo: string;
  recetaId?: number | null;
  recetaNombre?: string | null;
  fermentadorId: number;
  fermentadorNombre?: string | null;
  fechaElaboracion: string;
  estilo?: string | null;
  inoculo?: string | null;
  responsable?: string | null;
  diasEstimadosFermentacion: number;
  estado: string;
  observaciones?: string | null;
  fechaFinReal?: string | null;
}
