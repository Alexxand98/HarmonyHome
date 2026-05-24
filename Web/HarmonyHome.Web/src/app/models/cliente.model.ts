export interface Cliente {
  id: number;
  nombre: string;
  apellidos: string;
  telefono?: string;
  email?: string;
  direccion?: string;
  activo: boolean;
  fechaAlta: string;
}

export interface CreateCliente {
  nombre: string;
  apellidos: string;
  telefono?: string;
  email?: string;
  direccion?: string;
}