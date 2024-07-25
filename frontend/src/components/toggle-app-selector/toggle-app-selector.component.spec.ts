import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ToggleAppSelectorComponent } from './toggle-app-selector.component';

describe('ToggleAppSelectorComponent', () => {
  let component: ToggleAppSelectorComponent;
  let fixture: ComponentFixture<ToggleAppSelectorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ToggleAppSelectorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ToggleAppSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
