export interface LineaPedidoOrdenRecogida {
  id: number;
  productoId: number;
  productoReferencia: string;
  productoNombre: string;
  cantidad: number;
  precioUnitario: number;
  subtotal: number;
}

export interface OrdenRecogida {
  id: number;
  pedidoVentaId: number;
  fechaCreacion: string;
  estado: number;
  estadoNombre: string;
  usuarioAsignadoId?: string | null;
  usuarioAsignadoUserName?: string | null;
  usuarioAsignadoEmail?: string | null;
  observaciones?: string | null;
  clienteId: number;
  clienteNombreCompleto: string;
  totalPedido: number;
  lineasPedido: LineaPedidoOrdenRecogida[];
}