import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StockGestion } from './stock-gestion';

describe('StockGestion', () => {
  let component: StockGestion;
  let fixture: ComponentFixture<StockGestion>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StockGestion],
    }).compileComponents();

    fixture = TestBed.createComponent(StockGestion);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
