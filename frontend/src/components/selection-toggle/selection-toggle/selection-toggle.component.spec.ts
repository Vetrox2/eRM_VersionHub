import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectionToggleComponent } from './selection-toggle.component';

describe('SelectionToggleComponent', () => {
  let component: SelectionToggleComponent;
  let fixture: ComponentFixture<SelectionToggleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SelectionToggleComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SelectionToggleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
