import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecetaList } from './receta-list';

describe('RecetaList', () => {
  let component: RecetaList;
  let fixture: ComponentFixture<RecetaList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecetaList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RecetaList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
