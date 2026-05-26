import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { OrdenRecogida } from '../models/orden-recogida.model';
import { ResponseApi } from '../models/response-api.model';

@Injectable({
  providedIn: 'root'
})
export class OrdenRecogidaService {
  private readonly apiUrl = `${environment.apiUrl}/OrdenRecogida`;

  constructor(private http: HttpClient) {}

  getOrdenes(): Observable<ResponseApi<OrdenRecogida[]>> {
    return this.http.get<ResponseApi<OrdenRecogida[]>>(this.apiUrl);
  }
}