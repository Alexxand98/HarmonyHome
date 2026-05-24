import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { Cliente, CreateCliente } from '../models/cliente.model';
import { ResponseApi } from '../models/response-api.model';

@Injectable({
  providedIn: 'root'
})
export class ClienteService {
  private readonly apiUrl = `${environment.apiUrl}/Cliente`;

  constructor(private http: HttpClient) {}

  getClientesActivos(): Observable<ResponseApi<Cliente[]>> {
    return this.http.get<ResponseApi<Cliente[]>>(`${this.apiUrl}/activos`);
  }

  buscarClientes(texto: string): Observable<ResponseApi<Cliente[]>> {
    const params = new HttpParams().set('texto', texto);

    return this.http.get<ResponseApi<Cliente[]>>(`${this.apiUrl}/buscar`, { params });
  }

  crearCliente(cliente: CreateCliente): Observable<ResponseApi<Cliente>> {
    return this.http.post<ResponseApi<Cliente>>(this.apiUrl, cliente);
  }

  actualizarCliente(id: number, cliente: CreateCliente): Observable<ResponseApi<Cliente>> {
    return this.http.put<ResponseApi<Cliente>>(`${this.apiUrl}/${id}`, cliente);
  }

  eliminarCliente(id: number): Observable<ResponseApi<string>> {
    return this.http.delete<ResponseApi<string>>(`${this.apiUrl}/${id}`);
  }
}