import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { Auth } from '../../services/auth';
import { LoginRequest } from '../../models/auth.model';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  email = '';
  password = '';
  errorMessage = '';
  isLoading = false;

  constructor(
    private authService: Auth,
    private router: Router
  ) {}

  login(): void {
    this.errorMessage = '';

    const request: LoginRequest = {
      email: this.email,
      password: this.password
    };

    if (!request.email || !request.password) {
      this.errorMessage = 'Introduce email y contraseña.';
      return;
    }

    this.isLoading = true;

    this.authService.login(request).subscribe({
      next: response => {
        this.isLoading = false;

        if (response.isSuccess && response.result) {
          this.router.navigate(['/productos']);
          return;
        }

        this.errorMessage = response.errorMessages?.[0] ?? 'No se pudo iniciar sesión.';
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Error al conectar con la API.';
      }
    });
  }
}