import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FermentadorComponent } from './fermentador';

describe('Fermentador', () => {
  let component: FermentadorComponent;
  let fixture: ComponentFixture<FermentadorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FermentadorComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(FermentadorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
