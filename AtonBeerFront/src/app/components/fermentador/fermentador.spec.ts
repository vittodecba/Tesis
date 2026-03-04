import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Fermentador } from './fermentador';

describe('Fermentador', () => {
  let component: Fermentador;
  let fixture: ComponentFixture<Fermentador>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Fermentador]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Fermentador);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
