import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserAppPermissionsComponent } from './user-app-permissions.component';

describe('UserAppPermissionsComponent', () => {
  let component: UserAppPermissionsComponent;
  let fixture: ComponentFixture<UserAppPermissionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserAppPermissionsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserAppPermissionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
