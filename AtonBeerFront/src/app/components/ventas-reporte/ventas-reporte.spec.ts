import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VentasReporte } from './ventas-reporte';

describe('VentasReporte', () => {
  let component: VentasReporte;
  let fixture: ComponentFixture<VentasReporte>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VentasReporte]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VentasReporte);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
