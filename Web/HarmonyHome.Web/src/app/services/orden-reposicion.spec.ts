import { TestBed } from '@angular/core/testing';

import { OrdenReposicion } from './orden-reposicion';

describe('OrdenReposicion', () => {
  let service: OrdenReposicion;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OrdenReposicion);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
