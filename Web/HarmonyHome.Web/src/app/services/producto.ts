import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { Producto } from '../models/producto.model';
import { ResponseApi } from '../models/response-api.model';

@Injectable({
  providedIn: 'root'
})
export class ProductoService {
  private readonly apiUrl = `${environment.apiUrl}/Producto`;

  constructor(private http: HttpClient) {}

  getProductosHabilitados(): Observable<ResponseApi<Producto[]>> {
    return this.http.get<ResponseApi<Producto[]>>(`${this.apiUrl}/habilitados`);
  }

  buscarProductos(texto: string): Observable<ResponseApi<Producto[]>> {
    const params = new HttpParams().set('texto', texto);

    return this.http.get<ResponseApi<Producto[]>>(`${this.apiUrl}/buscar`, { params });
  }
}