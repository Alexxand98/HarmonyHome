import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';

import { OrdenRecogida } from '../../models/orden-recogida.model';
import { OrdenReposicion } from '../../models/orden-reposicion.model';

import { OrdenRecogidaService } from '../../services/orden-recogida';
import { OrdenReposicionService } from '../../services/orden-reposicion';

import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-ordenes',
  imports: [CommonModule, FormsModule],
  templateUrl: './ordenes.html',
  styleUrl: './ordenes.css'
})
export class Ordenes implements OnInit {
  ordenesRecogida: OrdenRecogida[] = [];
  ordenesReposicion: OrdenReposicion[] = [];

  textoBusquedaRecogida = '';
  textoBusquedaReposicion = '';

  isLoading = false;
  errorMessage = '';

  constructor(
    private ordenRecogidaService: OrdenRecogidaService,
    private ordenReposicionService: OrdenReposicionService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.cargarOrdenes();
  }

  cargarOrdenes(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.detectChanges();

    this.cargarOrdenesRecogida();
    this.cargarOrdenesReposicion();
  }

  cargarOrdenesRecogida(): void {
    this.ordenRecogidaService.getOrdenes().subscribe({
      next: response => {
        if (response.isSuccess && response.result) {
          this.ordenesRecogida = response.result;
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudieron cargar las órdenes de recogida';
        }

        this.finalizarCarga();
      },
      error: () => {
        this.errorMessage = 'Error al cargar las órdenes de recogida';
        this.finalizarCarga();
      }
    });
  }

  cargarOrdenesReposicion(): void {
    this.ordenReposicionService.getOrdenes().subscribe({
      next: response => {
        if (response.isSuccess && response.result) {
          this.ordenesReposicion = response.result;
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudieron cargar las órdenes de reposición';
        }

        this.finalizarCarga();
      },
      error: () => {
        this.errorMessage = 'Error al cargar las órdenes de reposición';
        this.finalizarCarga();
      }
    });
  }

  finalizarCarga(): void {
    this.isLoading = false;
    this.cdr.detectChanges();
  }

  get ordenesRecogidaFiltradas(): OrdenRecogida[] {
    const texto = this.textoBusquedaRecogida.trim().toLowerCase();

    if (!texto) {
      return this.ordenesRecogida;
    }

    return this.ordenesRecogida.filter(orden =>
      orden.id.toString().includes(texto) ||
      orden.pedidoVentaId.toString().includes(texto) ||
      orden.clienteNombreCompleto.toLowerCase().includes(texto) ||
      orden.estadoNombre.toLowerCase().includes(texto) ||
      orden.lineasPedido.some(linea =>
        linea.productoNombre.toLowerCase().includes(texto) ||
        linea.productoReferencia.toLowerCase().includes(texto)
      )
    );
  }

  get ordenesReposicionFiltradas(): OrdenReposicion[] {
    const texto = this.textoBusquedaReposicion.trim().toLowerCase();

    if (!texto) {
      return this.ordenesReposicion;
    }

    return this.ordenesReposicion.filter(orden =>
      orden.id.toString().includes(texto) ||
      orden.estadoNombre.toLowerCase().includes(texto) ||
      (orden.usuarioSolicitanteUserName ?? '').toLowerCase().includes(texto) ||
      (orden.usuarioPreparadorUserName ?? '').toLowerCase().includes(texto) ||
      orden.lineas.some(linea =>
        linea.productoNombre.toLowerCase().includes(texto) ||
        linea.productoReferencia.toLowerCase().includes(texto)
      )
    );
  }
}