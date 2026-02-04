import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StockGestion } from './stock-gestion';

describe('StockGestion', () => {
  let component: StockGestion;
  let fixture: ComponentFixture<StockGestion>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StockGestion], // standalone
    }).compileComponents();

    fixture = TestBed.createComponent(StockGestion);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should calculate totalAll', () => {
    const total = component.totalAll();
    expect(total).toBeGreaterThan(0);
  });

  it('should toggle expand', () => {
    const first = component.products[0].id;
    component.expandedId = '';
    component.toggleExpand(first);
    expect(component.expandedId).toBe(first);
    component.toggleExpand(first);
    expect(component.expandedId).toBe('');
  });

  it('should filter movements by tipo', () => {
    component.tipoMov = 'Venta';
    const filtered = component.filteredMovements();
    expect(filtered.every((m) => m.tipo === 'Venta')).toBeTrue();
  });
});
