import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChipDropdownComponent } from './chip-dropdown.component';

describe('ChipDropdownComponent', () => {
  let component: ChipDropdownComponent;
  let fixture: ComponentFixture<ChipDropdownComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChipDropdownComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChipDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
