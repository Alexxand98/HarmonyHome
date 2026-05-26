export interface LineaOrdenReposicionRequest {
  productoId: number;
  cantidadSolicitada: number;
}

export interface OrdenReposicionRequest {
  lineas: LineaOrdenReposicionRequest[];
  observaciones?: string;
}

export interface LineaOrdenReposicion {
  id: number;
  productoId: number;
  productoReferencia: string;
  productoNombre: string;
  cantidadSolicitada: number;
  cantidadPreparada: number;
}

export interface OrdenReposicion {
  id: number;
  fechaSolicitud: string;
  estado: number;
  estadoNombre: string;
  usuarioSolicitanteId: string;
  usuarioPreparadorId?: string | null;
  observaciones?: string | null;
  usuarioSolicitanteUserName?: string | null;
  usuarioSolicitanteEmail?: string | null;
  usuarioPreparadorUserName?: string | null;
  usuarioPreparadorEmail?: string | null;
  lineas: LineaOrdenReposicion[];
}