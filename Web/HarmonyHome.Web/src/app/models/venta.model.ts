export interface LineaVentaRequest {
    productoId: number;
    cantidad: number;
}

export interface VentaDirectaRequest {
    clienteId: number;
    lineas: LineaVentaRequest[];
    observaciones?: string | null;
}

export interface VentaMixtaResponse {
  tipoOperacion?: string;
  ventaDirectaId?: number;
  pedidoClienteId?: number;
  ordenRecogidaId?: number;
  mensaje?: string;
}