export interface Producto {
  id: number;
  referencia: string;
  nombre: string;
  descripcion?: string;
  categoria: string;
  precioCoste: number;
  precioVenta: number;
  stockMinimo: number;
  tipoTrazabilidad: number;
  tipoTrazabilidadNombre: string;
  habilitado: boolean;
  activo: boolean;
  fechaAlta: string;
  imagenUrl?: string | null;
  observaciones?: string | null;
}