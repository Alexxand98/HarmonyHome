import { TestBed } from '@angular/core/testing';

import { PedidoCliente } from './pedido-cliente';

describe('PedidoCliente', () => {
  let service: PedidoCliente;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PedidoCliente);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
