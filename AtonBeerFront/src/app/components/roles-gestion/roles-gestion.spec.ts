import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RolesGestion } from './roles-gestion';

describe('RolesGestion', () => {
  let component: RolesGestion;
  let fixture: ComponentFixture<RolesGestion>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RolesGestion]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RolesGestion);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
