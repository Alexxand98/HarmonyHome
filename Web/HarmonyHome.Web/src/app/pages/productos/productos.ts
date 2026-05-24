import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { Producto } from '../../models/producto.model';
import { StockResumen } from '../../models/stock.model';
import { ProductoService } from '../../services/producto';
import { StockService } from '../../services/stock';

@Component({
  selector: 'app-productos',
  imports: [CommonModule, FormsModule],
  templateUrl: './productos.html',
  styleUrl: './productos.css'
})
export class Productos implements OnInit {
  productos: Producto[] = [];
  stockSeleccionado: StockResumen | null = null;

  textoBusqueda = '';
  isLoading = false;
  isLoadingStock = false;
  errorMessage = '';
  stockErrorMessage = '';

  constructor(
    private productoService: ProductoService,
    private stockService: StockService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.cargarProductos();
  }

  cargarProductos(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.stockSeleccionado = null;
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

  buscarProductos(): void {
    const texto = this.textoBusqueda.trim();

    if (!texto) {
      this.cargarProductos();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.stockSeleccionado = null;
    this.cdr.detectChanges();

    this.productoService.buscarProductos(texto).subscribe({
      next: response => {
        this.isLoading = false;

        if (response.isSuccess && response.result) {
          this.productos = response.result;
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se encontraron productos.';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Error al buscar productos.';
        this.cdr.detectChanges();
      }
    });
  }

  limpiarBusqueda(): void {
    this.textoBusqueda = '';
    this.cargarProductos();
  }

  verStock(productoId: number): void {
    this.isLoadingStock = true;
    this.stockErrorMessage = '';
    this.stockSeleccionado = null;
    this.cdr.detectChanges();

    this.stockService.getResumenProducto(productoId).subscribe({
      next: response => {
        this.isLoadingStock = false;

        if (response.isSuccess && response.result) {
          this.stockSeleccionado = response.result;
        } else {
          this.stockErrorMessage = response.errorMessages?.[0] ?? 'No se pudo cargar el stock.';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoadingStock = false;
        this.stockErrorMessage = 'Error al cargar el stock del producto.';
        this.cdr.detectChanges();
      }
    });
  }
}