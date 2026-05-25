import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { PedidoClienteRequest } from '../models/pedido-cliente.model';
import { ResponseApi } from '../models/response-api.model';

@Injectable({
  providedIn: 'root'
})
export class PedidoClienteService {
  private readonly apiUrl = `${environment.apiUrl}/PedidoCliente`;

  constructor(private http: HttpClient) {}

  crearPedidoCliente(request: PedidoClienteRequest): Observable<ResponseApi<unknown>> {
    return this.http.post<ResponseApi<unknown>>(this.apiUrl, request);
  }
}