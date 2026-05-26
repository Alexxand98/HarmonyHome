import { CommonModule } from '@angular/common';
import { Component, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { Producto } from '../../models/producto.model';
import { StockResumen } from '../../models/stock.model';
import { OrdenReposicionRequest } from '../../models/orden-reposicion.model';

import { ProductoService } from '../../services/producto';
import { StockService } from '../../services/stock';
import { OrdenReposicionService } from '../../services/orden-reposicion';

@Component({
  selector: 'app-reposiciones',
  imports: [CommonModule, FormsModule],
  templateUrl: './reposiciones.html',
  styleUrl: './reposiciones.css'
})
export class Reposiciones {
  textoBusqueda = '';
  productos: Producto[] = [];

  productoSeleccionado: Producto | null = null;
  stockSeleccionado: StockResumen | null = null;

  cantidadPendienteReposicion = 0;
  stockAlmacenDisponible = 0;

  cantidadSolicitada = 1;
  observaciones = '';

  isLoading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private productoService: ProductoService,
    private stockService: StockService,
    private ordenReposicionService: OrdenReposicionService,
    private cdr: ChangeDetectorRef
  ) { }

  buscarProductos(): void {
    const texto = this.textoBusqueda.trim();

    if (!texto) {
      this.errorMessage = 'Introduce un texto para buscar productos';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.productos = [];
    this.productoSeleccionado = null;
    this.stockSeleccionado = null;
    this.cantidadPendienteReposicion = 0;
    this.stockAlmacenDisponible = 0;
    this.cdr.detectChanges();

    this.productoService.buscarProductos(texto).subscribe({
      next: response => {
        this.isLoading = false;

        if (response.isSuccess && response.result) {
          this.productos = response.result.filter(producto => producto.activo && producto.habilitado);
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se encontraron productos';
        }

        if (this.productos.length === 0 && !this.errorMessage) {
          this.errorMessage = 'No se encontraron productos habilitados';
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

  seleccionarProducto(producto: Producto): void {
    this.productoSeleccionado = producto;
    this.productos = [];
    this.textoBusqueda = `${producto.referencia} - ${producto.nombre}`;
    this.cantidadSolicitada = 1;
    this.cargarStock(producto.id);
  }

  cargarStock(productoId: number): void {
    this.stockService.getResumenProducto(productoId).subscribe({
      next: response => {
        if (response.isSuccess && response.result) {
          this.stockSeleccionado = response.result;
          this.calcularStockDisponible(productoId);
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudo cargar el stock';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Error al cargar el stock del producto';
        this.cdr.detectChanges();
      }
    });
  }

  calcularStockDisponible(productoId: number): void {
    this.cantidadPendienteReposicion = 0;
    this.stockAlmacenDisponible = this.stockSeleccionado?.stockAlmacen ?? 0;

    this.ordenReposicionService.getOrdenesPendientes().subscribe({
      next: response => {
        if (response.isSuccess && response.result) {
          for (const orden of response.result) {
            for (const linea of orden.lineas) {
              if (linea.productoId === productoId) {
                this.cantidadPendienteReposicion +=
                  linea.cantidadSolicitada - linea.cantidadPreparada;
              }
            }
          }

          this.stockAlmacenDisponible =
            (this.stockSeleccionado?.stockAlmacen ?? 0) - this.cantidadPendienteReposicion;

          if (this.stockAlmacenDisponible < 0) {
            this.stockAlmacenDisponible = 0;
          }
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Error al calcular el stock disponible';
        this.cdr.detectChanges();
      }
    });
  }

  crearReposicion(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.productoSeleccionado || !this.stockSeleccionado) {
      this.errorMessage = 'Selecciona un producto';
      return;
    }

    if (this.cantidadSolicitada < 1) {
      this.errorMessage = 'La cantidad debe ser mayor que cero';
      return;
    }

    if (this.cantidadSolicitada > this.stockAlmacenDisponible) {
      this.errorMessage =
        `No hay suficiente stock disponible en almacén. Ya hay ${this.cantidadPendienteReposicion} unidades pendientes de reposición`;
      return;
    }

    const request: OrdenReposicionRequest = {
      lineas: [
        {
          productoId: this.productoSeleccionado.id,
          cantidadSolicitada: this.cantidadSolicitada
        }
      ],
      observaciones: this.observaciones || 'Solicitud de reposición desde Angular'
    };

    this.isLoading = true;
    this.cdr.detectChanges();

    this.ordenReposicionService.crearOrdenReposicion(request).subscribe({
      next: response => {
        this.isLoading = false;

        if (response.isSuccess) {
          this.successMessage = 'Reposición solicitada correctamente';
          this.limpiarFormulario();
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudo solicitar la reposición';
        }

        this.cdr.detectChanges();
      },
      error: error => {
        this.isLoading = false;
        this.errorMessage =
          error.error?.errorMessages?.[0] ??
          error.error?.title ??
          error.message ??
          'Error al solicitar la reposición';

        this.cdr.detectChanges();
      }
    });
  }

  limpiarFormulario(): void {
    this.textoBusqueda = '';
    this.productos = [];
    this.productoSeleccionado = null;
    this.stockSeleccionado = null;
    this.cantidadPendienteReposicion = 0;
    this.stockAlmacenDisponible = 0;
    this.cantidadSolicitada = 1;
    this.observaciones = '';
  }
}