import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Reposiciones } from './reposiciones';

describe('Reposiciones', () => {
  let component: Reposiciones;
  let fixture: ComponentFixture<Reposiciones>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Reposiciones],
    }).compileComponents();

    fixture = TestBed.createComponent(Reposiciones);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
