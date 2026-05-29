import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { CarritoLinea } from '../../models/carrito.model';
import { Cliente } from '../../models/cliente.model';
import { VentaDirectaRequest } from '../../models/venta.model';

import { CarritoService } from '../../services/carrito';
import { ClienteService } from '../../services/cliente';
import { VentaService } from '../../services/venta';

import { PedidoClienteRequest } from '../../models/pedido-cliente.model';
import { PedidoClienteService } from '../../services/pedido-cliente';
import { StockService } from '../../services/stock';

@Component({
  selector: 'app-venta',
  imports: [CommonModule, FormsModule],
  templateUrl: './venta.html',
  styleUrl: './venta.css'
})
export class Venta implements OnInit {
  lineas: CarritoLinea[] = [];

  textoBusquedaCliente = '';
  clientesEncontrados: Cliente[] = [];
  clienteSeleccionado: Cliente | null = null;

  observaciones = '';

  isLoading = false;
  isSearchingClient = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private carritoService: CarritoService,
    private clienteService: ClienteService,
    private ventaService: VentaService,
    private pedidoClienteService: PedidoClienteService,
    private stockService: StockService,
    private cdr: ChangeDetectorRef

  ) { }

  ngOnInit(): void {
    this.cargarCarrito();
  }

  cargarCarrito(): void {
    this.lineas = this.carritoService.obtenerCarrito();
    this.cdr.detectChanges();
  }

  buscarCliente(): void {
    const texto = this.textoBusquedaCliente.trim();

    const esTelefono = /^[69][0-9]{8}$/.test(texto);
    const esEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(texto);

    if (!texto) {
      this.errorMessage = 'Introduce el email o teléfono del cliente';
      return;
    }

    if (!esTelefono && !esEmail) {
      this.errorMessage = 'Introduce un teléfono válido de 9 dígitos que empiece por 6 o 9, o un email válido';
      return;
    }

    this.isSearchingClient = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.clientesEncontrados = [];
    this.clienteSeleccionado = null;
    this.cdr.detectChanges();

    this.clienteService.buscarClientes(texto).subscribe({
      next: response => {
        this.isSearchingClient = false;

        if (response.isSuccess && response.result) {
          this.clientesEncontrados = response.result.filter(cliente => cliente.activo);
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se encontraron clientes';
        }

        if (this.clientesEncontrados.length === 0 && !this.errorMessage) {
          this.errorMessage = 'No se encontraron clientes activos';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.isSearchingClient = false;
        this.errorMessage = 'Error al buscar cliente';
        this.cdr.detectChanges();
      }
    });
  }

  seleccionarCliente(cliente: Cliente): void {
    this.clienteSeleccionado = cliente;
    this.clientesEncontrados = [];
    this.textoBusquedaCliente = `${cliente.nombre} ${cliente.apellidos}`;
    this.errorMessage = '';
  }

  quitarCliente(): void {
    this.clienteSeleccionado = null;
    this.textoBusquedaCliente = '';
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

  confirmarVenta(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.lineas.length === 0) {
      this.errorMessage = 'El carrito está vacío';
      return;
    }

    if (!this.clienteSeleccionado) {
      this.errorMessage = 'Busca y selecciona un cliente para realizar la venta';
      return;
    }

    const request: VentaDirectaRequest = {
      clienteId: this.clienteSeleccionado.id,
      lineas: this.lineas.map(linea => ({
        productoId: linea.productoId,
        cantidad: linea.cantidad
      })),
      observaciones: this.observaciones || 'Operación confirmada desde Angular'
    };

    this.isLoading = true;
    this.cdr.detectChanges();

    this.ventaService.crearVentaMixta(request).subscribe({
      next: response => {
        this.isLoading = false;

        if (response.isSuccess) {
          const tipoOperacion = response.result?.tipoOperacion;

          if (tipoOperacion === 'VentaDirecta') {
            this.successMessage = 'Venta realizada desde tienda';
          } else if (tipoOperacion === 'PedidoCliente') {
            this.successMessage = 'Pedido de cliente creado y enviado a logística';
          } else if (tipoOperacion === 'Mixta') {
            this.successMessage = 'Operación mixta realizada: venta en tienda y orden de recogida creada';
          } else {
            this.successMessage = response.result?.mensaje ?? 'Operación confirmada correctamente';
          }

          this.vaciarCarrito();
          this.observaciones = '';
          this.quitarCliente();
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudo realizar la venta';
        }

        this.cdr.detectChanges();
      },
      error: error => {
        this.isLoading = false;
        this.errorMessage =
          error.error?.errorMessages?.[0] ??
          error.error?.title ??
          error.message ??
          'Error al realizar la venta';

        this.cdr.detectChanges();
      }
    });
  }

  crearPedidoCliente(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.lineas.length === 0) {
      this.errorMessage = 'El carrito está vacío';
      return;
    }

    if (!this.clienteSeleccionado) {
      this.errorMessage = 'Busca y selecciona un cliente para crear el pedido';
      return;
    }

    const lineaInvalida = this.lineas.find(linea => linea.cantidad < 1);

    if (lineaInvalida) {
      this.errorMessage = 'Todas las cantidades deben ser mayores que cero';
      return;
    }

    this.isLoading = true;
    this.cdr.detectChanges();

    let comprobacionesPendientes = this.lineas.length;
    let stockCorrecto = true;

    for (const linea of this.lineas) {
      this.stockService.getResumenProducto(linea.productoId).subscribe({
        next: response => {
          comprobacionesPendientes--;

          if (!response.isSuccess || !response.result) {
            stockCorrecto = false;
            this.errorMessage = `No se pudo comprobar el stock de ${linea.nombre}`;
          } else if (linea.cantidad > response.result.stockAlmacen) {
            stockCorrecto = false;
            this.errorMessage = `Stock insuficiente en almacén para ${linea.nombre}`;
          }

          if (comprobacionesPendientes === 0) {
            if (!stockCorrecto) {
              this.isLoading = false;
              this.cdr.detectChanges();
              return;
            }

            this.enviarPedidoCliente();
          }
        },
        error: () => {
          comprobacionesPendientes--;
          stockCorrecto = false;
          this.errorMessage = `Error al comprobar stock de ${linea.nombre}`;

          if (comprobacionesPendientes === 0) {
            this.isLoading = false;
            this.cdr.detectChanges();
          }
        }
      });
    }
  }

  private enviarPedidoCliente(): void {
    if (!this.clienteSeleccionado) {
      this.isLoading = false;
      this.errorMessage = 'Selecciona un cliente para crear el pedido';
      this.cdr.detectChanges();
      return;
    }

    const request: PedidoClienteRequest = {
      clienteId: this.clienteSeleccionado.id,
      lineas: this.lineas.map(linea => ({
        productoId: linea.productoId,
        cantidad: linea.cantidad
      })),
      observaciones: this.observaciones || 'Pedido cliente desde Angular'
    };

    this.pedidoClienteService.crearPedidoCliente(request).subscribe({
      next: response => {
        this.isLoading = false;

        if (response.isSuccess) {
          this.successMessage = 'Pedido de cliente creado correctamente';
          this.vaciarCarrito();
          this.observaciones = '';
          this.quitarCliente();
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudo crear el pedido de cliente';
        }

        this.cdr.detectChanges();
      },
      error: error => {
        this.isLoading = false;
        this.errorMessage =
          error.error?.errorMessages?.[0] ??
          error.error?.title ??
          error.message ??
          'Error al crear el pedido de cliente';

        this.cdr.detectChanges();
      }
    });
  }

}