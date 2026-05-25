import { Injectable } from '@angular/core';

import { CarritoLinea } from '../models/carrito.model';

@Injectable({
  providedIn: 'root'
})
export class CarritoService {
  private readonly carritoKey = 'hh_carrito';

  obtenerCarrito(): CarritoLinea[] {
    const carritoJson = localStorage.getItem(this.carritoKey);

    if (!carritoJson) {
      return [];
    }

    try {
      return JSON.parse(carritoJson) as CarritoLinea[];
    } catch {
      return [];
    }
  }

  guardarCarrito(lineas: CarritoLinea[]): void {
    localStorage.setItem(this.carritoKey, JSON.stringify(lineas));
  }

  agregarProducto(linea: CarritoLinea): void {
    const carrito = this.obtenerCarrito();

    const lineaExistente = carrito.find(item => item.productoId === linea.productoId);

    if (lineaExistente) {

      lineaExistente.cantidad += linea.cantidad;

    } else {

      carrito.push(linea);
    }

    this.guardarCarrito(carrito);
  }

  eliminarProducto(productoId: number): void {

    const carrito = this.obtenerCarrito().filter(item => item.productoId !== productoId);

    this.guardarCarrito(carrito);
  }

  vaciarCarrito(): void {
    
    localStorage.removeItem(this.carritoKey);
  }
}