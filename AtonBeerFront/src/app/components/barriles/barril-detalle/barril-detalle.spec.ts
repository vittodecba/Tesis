import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BarrilDetalleComponent } from './barril-detalle';

describe('BarrilDetalleComponent', () => {
  let component: BarrilDetalleComponent;
  let fixture: ComponentFixture<BarrilDetalleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BarrilDetalleComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BarrilDetalleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});