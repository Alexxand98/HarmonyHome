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

  mostrarFormulario = false;
  clienteEditandoId: number | null = null;

  clienteForm: CreateCliente = {
    nombre: '',
    apellidos: '',
    telefono: '',
    email: '',
    direccion: ''
  };

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
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudieron cargar los clientes';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Error al cargar clientes desde la API';
        this.cdr.detectChanges();
      }
    });
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
          this.errorMessage = response.errorMessages?.[0] ?? 'No se encontraron clientes';
        }

        if (this.clientes.length === 0 && !this.errorMessage) {
          this.errorMessage = 'No se encontraron clientes activos';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Error al buscar clientes';
        this.cdr.detectChanges();
      }
    });
  }

  limpiarBusqueda(): void {
    this.textoBusqueda = '';
    this.cargarClientes();
  }

  nuevoCliente(): void {
    this.limpiarFormulario();
    this.mostrarFormulario = true;
  }

  guardarCliente(): void {
    this.errorMessage = '';
    this.successMessage = '';

    const nombre = this.clienteForm.nombre.trim();
    const apellidos = this.clienteForm.apellidos.trim();
    const telefono = this.clienteForm.telefono?.trim() ?? '';
    const email = this.clienteForm.email?.trim() ?? '';
    const direccion = this.clienteForm.direccion?.trim() ?? '';

    const textoValido = /^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/;
    const telefonoValido = /^[69][0-9]{8}$/;
    const emailValido = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!nombre) {
      this.errorMessage = 'Introduce el nombre del cliente';
      return;
    }

    if (!textoValido.test(nombre)) {
      this.errorMessage = 'El nombre solo puede contener letras';
      return;
    }

    if (!apellidos) {
      this.errorMessage = 'Introduce los apellidos del cliente';
      return;
    }

    if (!textoValido.test(apellidos)) {
      this.errorMessage = 'Los apellidos solo pueden contener letras';
      return;
    }

    if (!telefono) {
      this.errorMessage = 'Introduce el teléfono del cliente';
      return;
    }

    if (!telefonoValido.test(telefono)) {
      this.errorMessage = 'El teléfono debe tener 9 dígitos y empezar por 6 o 9';
      return;
    }

    if (!email) {
      this.errorMessage = 'Introduce el email del cliente';
      return;
    }

    if (!emailValido.test(email)) {
      this.errorMessage = 'Introduce un email válido';
      return;
    }

    if (!direccion) {
      this.errorMessage = 'Introduce la dirección del cliente';
      return;
    }

    const clienteParaEnviar: CreateCliente = {
      nombre,
      apellidos,
      telefono,
      email,
      direccion
    };

    if (this.clienteEditandoId) {
      this.actualizarCliente(clienteParaEnviar);
    } else {
      this.crearCliente(clienteParaEnviar);
    }
  }

  crearCliente(clienteParaEnviar: CreateCliente): void {
    this.isLoading = true;
    this.cdr.detectChanges();

    this.clienteService.crearCliente(clienteParaEnviar).subscribe({
      next: response => {
        this.isLoading = false;

        if (response.isSuccess) {
          this.successMessage = 'Cliente creado correctamente';
          this.cerrarFormulario();
          this.cargarClientes();
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudo crear el cliente';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Error al crear el cliente';
        this.cdr.detectChanges();
      }
    });
  }

  actualizarCliente(clienteParaEnviar: CreateCliente): void {
    if (!this.clienteEditandoId) {
      return;
    }

    this.isLoading = true;
    this.cdr.detectChanges();

    this.clienteService.actualizarCliente(this.clienteEditandoId, clienteParaEnviar).subscribe({
      next: response => {
        this.isLoading = false;

        if (response.isSuccess) {
          this.successMessage = 'Cliente actualizado correctamente';
          this.cerrarFormulario();
          this.cargarClientes();
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudo actualizar el cliente';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Error al actualizar el cliente';
        this.cdr.detectChanges();
      }
    });
  }

  editarCliente(cliente: Cliente): void {
    this.clienteEditandoId = cliente.id;
    this.mostrarFormulario = true;

    this.clienteForm = {
      nombre: cliente.nombre,
      apellidos: cliente.apellidos,
      telefono: cliente.telefono ?? '',
      email: cliente.email ?? '',
      direccion: cliente.direccion ?? ''
    };
  }

  eliminarCliente(cliente: Cliente): void {
    const confirmar = confirm(`¿Eliminar o desactivar a ${cliente.nombre} ${cliente.apellidos}?`);

    if (!confirmar) {
      return;
    }

    this.clienteService.eliminarCliente(cliente.id).subscribe({
      next: response => {
        if (response.isSuccess) {
          this.successMessage = 'Cliente eliminado o desactivado correctamente';
          this.cargarClientes();
        } else {
          this.errorMessage = response.errorMessages?.[0] ?? 'No se pudo eliminar el cliente';
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Error al eliminar el cliente';
        this.cdr.detectChanges();
      }
    });
  }

  cerrarFormulario(): void {
    this.limpiarFormulario();
    this.mostrarFormulario = false;
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
}