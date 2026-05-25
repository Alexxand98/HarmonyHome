import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { ResponseApi } from '../models/response-api.model';
import { VentaDirectaRequest } from '../models/venta.model';

@Injectable({
  providedIn: 'root'
})
export class VentaService {
  private readonly apiUrl = `${environment.apiUrl}/Venta`;

  constructor(private http: HttpClient) { }

  crearVentaDirecta(request: VentaDirectaRequest): Observable<ResponseApi<unknown>> {
    return this.http.post<ResponseApi<unknown>>(`${this.apiUrl}/directa`, request);
  }
}