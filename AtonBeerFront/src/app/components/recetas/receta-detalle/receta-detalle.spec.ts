import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecetaDetalle } from './receta-detalle';

describe('RecetaDetalle', () => {
  let component: RecetaDetalle;
  let fixture: ComponentFixture<RecetaDetalle>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecetaDetalle]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RecetaDetalle);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
