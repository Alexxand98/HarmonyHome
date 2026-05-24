import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { environment } from '../../environments/environment';
import { LoginRequest, LoginResponse, AuthUser } from '../models/auth.model';
import { ResponseApi } from '../models/response-api.model';

@Injectable({
  providedIn: 'root'
})
export class Auth {
  private readonly tokenKey = 'hh_token';
  private readonly userKey = 'hh_user';

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<ResponseApi<LoginResponse>> {
    return this.http
      .post<ResponseApi<LoginResponse>>(`${environment.apiUrl}/Auth/login`, request)
      .pipe(
        tap(response => {
          if (response.isSuccess && response.result) {
            this.saveSession(response.result);
          }
        })
      );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getUser(): AuthUser | null {
    const userJson = localStorage.getItem(this.userKey);

    if (!userJson || userJson === 'undefined') {
      return null;
    }

    try {
      return JSON.parse(userJson) as AuthUser;
    } catch {
      return null;
    }
  }

  getRole(): string | null {
    return this.getUser()?.role ?? null;
  }

  private saveSession(response: LoginResponse): void {
    const user: AuthUser = {
      id: response.id,
      userName: response.userName,
      email: response.email,
      role: response.role
    };

    localStorage.setItem(this.tokenKey, response.token);
    localStorage.setItem(this.userKey, JSON.stringify(user));
  }
}