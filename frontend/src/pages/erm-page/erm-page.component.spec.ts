import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ErmPageComponent } from './erm-page.component';

describe('ErmPageComponent', () => {
  let component: ErmPageComponent;
  let fixture: ComponentFixture<ErmPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ErmPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ErmPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
