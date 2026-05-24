import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';

import { Producto } from '../../models/producto.model';
import { ProductoService } from '../../services/producto';

@Component({
  selector: 'app-productos',
  imports: [CommonModule],
  templateUrl: './productos.html',
  styleUrl: './productos.css'
})
export class Productos implements OnInit {
  productos: Producto[] = [];
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
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudieron cargar los productos.';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Error al cargar productos desde la API.';
        this.cdr.detectChanges();
      }
    });
  }
}