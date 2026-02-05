import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StockGestion } from './stock-gestion';
import { StockService } from '../../services/stock.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { FormsModule } from '@angular/forms';

describe('StockGestion', () => {
  let component: StockGestion;
  let fixture: ComponentFixture<StockGestion>;
  let stockServiceMock: any;

  beforeEach(async () => {
    // Creamos un simulador del servicio para que la prueba no intente llamar a la API real
    stockServiceMock = {
      getProductos: jasmine
        .createSpy('getProductos')
        .and.returnValue(
          of([
            { id: 1, formato: 'Barril', estilo: 'Rubia', stockActual: 10, unidadMedida: 'Litro' },
          ]),
        ),
      getMovimientos: jasmine.createSpy('getMovimientos').and.returnValue(of([])),
    };

    await TestBed.configureTestingModule({
      imports: [StockGestion, HttpClientTestingModule, FormsModule],
      providers: [{ provide: StockService, useValue: stockServiceMock }],
    }).compileComponents();

    fixture = TestBed.createComponent(StockGestion);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería agrupar productos correctamente al cargar', () => {
    // Al iniciar, el mock devuelve 1 producto, que se agrupa en 1 formato
    expect(component.products.length).toBe(1);
    expect(component.products[0].nombre).toBe('Barril');
  });

  it('debería alternar la expansión de una card', () => {
    const id = 'barril';
    component.toggleExpand(id);
    expect(component.expandedId).toBe(id);
    component.toggleExpand(id);
    expect(component.expandedId).toBe('');
  });
});
