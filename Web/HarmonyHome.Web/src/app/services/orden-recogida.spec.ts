import { TestBed } from '@angular/core/testing';

import { OrdenRecogida } from './orden-recogida';

describe('OrdenRecogida', () => {
  let service: OrdenRecogida;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OrdenRecogida);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
