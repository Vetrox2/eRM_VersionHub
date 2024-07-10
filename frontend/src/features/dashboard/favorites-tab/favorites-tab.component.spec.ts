import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FavoritesTabComponent } from './favorites-tab.component';

describe('FavoritesTabComponent', () => {
  let component: FavoritesTabComponent;
  let fixture: ComponentFixture<FavoritesTabComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FavoritesTabComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FavoritesTabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
