import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Insumo } from './insumo';

describe('Insumo', () => {
  let component: Insumo;
  let fixture: ComponentFixture<Insumo>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Insumo]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Insumo);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
