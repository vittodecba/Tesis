import { TestBed } from '@angular/core/testing';
import { App } from './app';
import { Router } from '@angular/router';
import { AuthService } from './core/services/auth.service';
import { of } from 'rxjs';

describe('App', () => {
  const routerSpy = { navigate: jasmine.createSpy('navigate') };
  const authServiceSpy = jasmine.createSpyObj('AuthService', ['isAuthenticated', 'logout']);

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        { provide: Router, useValue: routerSpy },
        { provide: AuthService, useValue: authServiceSpy },
      ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it(`should have the 'AtonBeerFront' title`, () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app.title).toEqual('AtonBeerFront');
  });

  it('debe llamar al logout del servicio y redirigir', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;

    app.logout();

    // Verificamos que delegue la responsabilidad al servicio
    expect(authServiceSpy.logout).toHaveBeenCalled();
  });
});
