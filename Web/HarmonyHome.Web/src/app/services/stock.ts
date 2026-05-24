import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { ResponseApi } from '../models/response-api.model';
import { StockResumen } from '../models/stock.model';

@Injectable({
  providedIn: 'root'
})
export class StockService {
  private readonly apiUrl = `${environment.apiUrl}/Stock`;

  constructor(private http: HttpClient) {}

  getResumenProducto(productoId: number): Observable<ResponseApi<StockResumen>> {
    return this.http.get<ResponseApi<StockResumen>>(`${this.apiUrl}/resumen/producto/${productoId}`);
  }
}