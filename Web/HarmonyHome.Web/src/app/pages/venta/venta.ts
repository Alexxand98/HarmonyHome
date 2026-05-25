import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { CarritoLinea } from '../../models/carrito.model';
import { CarritoService } from '../../services/carrito';

@Component({
  selector: 'app-venta',
  imports: [CommonModule, FormsModule],
  templateUrl: './venta.html',
  styleUrl: './venta.css'
})
export class Venta implements OnInit {
  lineas: CarritoLinea[] = [];

  constructor(
    private carritoService: CarritoService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.cargarCarrito();
  }

  cargarCarrito(): void {
    this.lineas = this.carritoService.obtenerCarrito();
    this.cdr.detectChanges();
  }

  actualizarCantidad(linea: CarritoLinea): void {
    if (linea.cantidad < 1) {
      linea.cantidad = 1;
    }

    this.carritoService.guardarCarrito(this.lineas);
  }

  eliminarLinea(productoId: number): void {
    this.carritoService.eliminarProducto(productoId);
    this.cargarCarrito();
  }

  vaciarCarrito(): void {
    this.carritoService.vaciarCarrito();
    this.cargarCarrito();
  }

  calcularTotal(): number {
    return this.lineas.reduce((total, linea) => total + linea.precioVenta * linea.cantidad, 0);
  }
}