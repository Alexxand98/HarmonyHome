export interface LineaVentaRequest {
    productoId: number;
    cantidad: number;
}

export interface VentaDirectaRequest {
    clienteId: number;
    lineas: LineaVentaRequest[];
    observaciones?: string | null;
}