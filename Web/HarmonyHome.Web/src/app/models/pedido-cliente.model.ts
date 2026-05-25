export interface LineaPedidoClienteRequest {
  productoId: number;
  cantidad: number;
}

export interface PedidoClienteRequest {
  clienteId: number;
  lineas: LineaPedidoClienteRequest[];
  observaciones?: string;
}