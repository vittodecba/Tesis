import { TestBed } from '@angular/core/testing';

import { Fermentador } from './fermentador';

describe('Fermentador', () => {
  let service: Fermentador;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Fermentador);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
