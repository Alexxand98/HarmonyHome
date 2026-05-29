import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { ResponseApi } from '../models/response-api.model';
import { VentaDirectaRequest, VentaMixtaResponse } from '../models/venta.model';
@Injectable({
  providedIn: 'root'
})
export class VentaService {
  private readonly apiUrl = `${environment.apiUrl}/Venta`;

  constructor(private http: HttpClient) { }

  crearVentaMixta(request: VentaDirectaRequest): Observable<ResponseApi<VentaMixtaResponse>> {
    return this.http.post<ResponseApi<VentaMixtaResponse>>(`${this.apiUrl}/mixta`, request);
  }
}