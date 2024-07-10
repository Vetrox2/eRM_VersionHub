import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomCheckboxChipComponent } from './custom-checkbox-chip.component';

describe('CustomCheckboxChipComponent', () => {
  let component: CustomCheckboxChipComponent;
  let fixture: ComponentFixture<CustomCheckboxChipComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomCheckboxChipComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CustomCheckboxChipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
