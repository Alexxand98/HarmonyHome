import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { OrdenReposicion, OrdenReposicionRequest } from '../models/orden-reposicion.model';
import { ResponseApi } from '../models/response-api.model';

@Injectable({
  providedIn: 'root'
})
export class OrdenReposicionService {
  private readonly apiUrl = `${environment.apiUrl}/OrdenReposicion`;

  constructor(private http: HttpClient) {}

  crearOrdenReposicion(request: OrdenReposicionRequest): Observable<ResponseApi<unknown>> {
    return this.http.post<ResponseApi<unknown>>(this.apiUrl, request);
  }

  getOrdenes(): Observable<ResponseApi<OrdenReposicion[]>> {
    return this.http.get<ResponseApi<OrdenReposicion[]>>(this.apiUrl);
  }

  getOrdenesPendientes(): Observable<ResponseApi<OrdenReposicion[]>> {
    return this.http.get<ResponseApi<OrdenReposicion[]>>(`${this.apiUrl}/pendientes`);
  }
}