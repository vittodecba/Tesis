import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StockGestion } from './stock-gestion';
import { StockService } from '../../services/stock.service';
import { of } from 'rxjs';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('StockGestion', () => {
  let component: StockGestion;
  let fixture: ComponentFixture<StockGestion>;
  let stockServiceSpy: jasmine.SpyObj<StockService>;

  beforeEach(async () => {
    // Creamos un "espía" para el servicio para no pegarle a la API real durante el test
    const spy = jasmine.createSpyObj('StockService', ['getProductos', 'getMovimientos']);

    // Configuramos que el espía devuelva datos de ejemplo (mock)
    spy.getProductos.and.returnValue(
      of([
        {
          id: 1,
          nombre: 'B50L IPA',
          estilo: 'IPA',
          formato: 'Barril 50L',
          unidadMedida: 'Litro',
          stockActual: 10,
        },
      ]),
    );
    spy.getMovimientos.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [StockGestion, HttpClientTestingModule],
      providers: [{ provide: StockService, useValue: spy }],
    }).compileComponents();

    fixture = TestBed.createComponent(StockGestion);
    component = fixture.componentInstance;
    stockServiceSpy = TestBed.inject(StockService) as jasmine.SpyObj<StockService>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should group products by format on init', () => {
    // Verificamos que la lógica de agrupación funcionó
    expect(component.formatosAgrupados.length).toBe(1);
    expect(component.formatosAgrupados[0].formato).toBe('Barril 50L');
    expect(component.formatosAgrupados[0].items[0].estilo).toBe('IPA');
  });

  it('should open movement modal with correct product', () => {
    const productoTest = { id: 1, nombre: 'B50L IPA' };
    component.openMovModal(productoTest);

    expect(component.movModalOpen).toBeTrue();
    expect(component.movProductoSeleccionado.id).toBe(1);
  });
});
