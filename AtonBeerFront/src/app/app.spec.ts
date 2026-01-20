import { TestBed } from '@angular/core/testing';
import { App } from './app';
import { Router } from '@angular/router';

describe('App', () => {
  // Creamos un "espía" del router para ver si lo llaman
  const routerSpy = { navigate: jasmine.createSpy('navigate') };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        { provide: Router, useValue: routerSpy }
      ]
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

  it('debe borrar el token y redirigir al cerrar sesion', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    spyOn(localStorage, 'removeItem');

    app.logout();

    // Verificamos las dos cosas:
    expect(localStorage.removeItem).toHaveBeenCalledWith('token'); // Que borre token
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);   // Que navegue a login
  });
});