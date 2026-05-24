import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { Producto } from '../../models/producto.model';
import { ProductoService } from '../../services/producto';

@Component({
  selector: 'app-productos',
  imports: [CommonModule, FormsModule],
  templateUrl: './productos.html',
  styleUrl: './productos.css'
})
export class Productos implements OnInit {
  productos: Producto[] = [];
  textoBusqueda = '';
  isLoading = false;
  errorMessage = '';

  constructor(
    private productoService: ProductoService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.cargarProductos();
  }

  cargarProductos(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.detectChanges();

    this.productoService.getProductosHabilitados().subscribe({
      next: response => {
        this.isLoading = false;

        if (response.isSuccess && response.result) {
          this.productos = response.result;
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudieron cargar los productos';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Error al cargar productos desde la API';
        this.cdr.detectChanges();
      }
    });
  }

  buscarProductos(): void {
    const texto = this.textoBusqueda.trim();

    if (!texto) {
      this.cargarProductos();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.detectChanges();

    this.productoService.buscarProductos(texto).subscribe({
      next: response => {
        this.isLoading = false;

        if (response.isSuccess && response.result) {
          this.productos = response.result;
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se encontraron productos';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Error al buscar productos';
        this.cdr.detectChanges();
      }
    });
  }

  limpiarBusqueda(): void {
    this.textoBusqueda = '';
    this.cargarProductos();
  }
}