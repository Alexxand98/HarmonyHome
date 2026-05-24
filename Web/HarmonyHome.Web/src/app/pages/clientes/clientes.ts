import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { Cliente, CreateCliente } from '../../models/cliente.model';
import { ClienteService } from '../../services/cliente';

@Component({
  selector: 'app-clientes',
  imports: [CommonModule, FormsModule],
  templateUrl: './clientes.html',
  styleUrl: './clientes.css'
})
export class Clientes implements OnInit {
  clientes: Cliente[] = [];

  textoBusqueda = '';

  clienteForm: CreateCliente = {
    nombre: '',
    apellidos: '',
    telefono: '',
    email: '',
    direccion: ''
  };

  clienteEditandoId: number | null = null;

  isLoading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private clienteService: ClienteService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.cargarClientes();
  }

  cargarClientes(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.cdr.detectChanges();

    this.clienteService.getClientesActivos().subscribe({
      next: response => {
        this.isLoading = false;

        if (response.isSuccess && response.result) {
          this.clientes = response.result;
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudieron cargar los clientes.';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Error al cargar clientes desde la API.';
        this.cdr.detectChanges();
      }
    });
  }

  guardarCliente(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.clienteForm.nombre || !this.clienteForm.apellidos) {
      this.errorMessage = 'Nombre y apellidos son obligatorios.';
      return;
    }

    if (this.clienteEditandoId) {
      this.actualizarCliente();
    } else {
      this.crearCliente();
    }
  }

  crearCliente(): void {
    this.clienteService.crearCliente(this.clienteForm).subscribe({
      next: response => {
        if (response.isSuccess) {
          this.successMessage = 'Cliente creado correctamente.';
          this.limpiarFormulario();
          this.cargarClientes();
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudo crear el cliente.';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Error al crear el cliente.';
        this.cdr.detectChanges();
      }
    });
  }

  actualizarCliente(): void {
    if (!this.clienteEditandoId) {
      return;
    }

    this.clienteService.actualizarCliente(this.clienteEditandoId, this.clienteForm).subscribe({
      next: response => {
        if (response.isSuccess) {
          this.successMessage = 'Cliente actualizado correctamente.';
          this.limpiarFormulario();
          this.cargarClientes();
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudo actualizar el cliente.';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Error al actualizar el cliente.';
        this.cdr.detectChanges();
      }
    });
  }

  editarCliente(cliente: Cliente): void {
    this.clienteEditandoId = cliente.id;

    this.clienteForm = {
      nombre: cliente.nombre,
      apellidos: cliente.apellidos,
      telefono: cliente.telefono ?? '',
      email: cliente.email ?? '',
      direccion: cliente.direccion ?? ''
    };
  }

  eliminarCliente(cliente: Cliente): void {
    const confirmar = confirm(`¿Eliminar o desactivar al cliente ${cliente.nombre} ${cliente.apellidos}?`);

    if (!confirmar) {
      return;
    }

    this.clienteService.eliminarCliente(cliente.id).subscribe({
      next: response => {
        if (response.isSuccess) {
          this.successMessage = 'Cliente eliminado o desactivado correctamente.';
          this.cargarClientes();
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudo eliminar el cliente.';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Error al eliminar el cliente.';
        this.cdr.detectChanges();
      }
    });
  }

  limpiarFormulario(): void {
    this.clienteEditandoId = null;

    this.clienteForm = {
      nombre: '',
      apellidos: '',
      telefono: '',
      email: '',
      direccion: ''
    };
  }

  buscarClientes(): void {
    const texto = this.textoBusqueda.trim();

    if (!texto) {
      this.cargarClientes();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.cdr.detectChanges();

    this.clienteService.buscarClientes(texto).subscribe({
      next: response => {
        this.isLoading = false;

        if (response.isSuccess && response.result) {

          this.clientes = response.result.filter(cliente => cliente.activo);
          
        } else {

          this.errorMessage = response.errorMessages?.[0] ?? 'No se encontraron clientes.';
          
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Error al buscar clientes.';
        this.cdr.detectChanges();
      }
    });
  }

  limpiarBusqueda(): void {
    this.textoBusqueda = '';
    this.cargarClientes();
  }
}