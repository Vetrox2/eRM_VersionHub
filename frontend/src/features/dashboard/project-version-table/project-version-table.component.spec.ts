import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectVersionTableComponent } from './project-version-table.component';

describe('ProjectVersionTableComponent', () => {
  let component: ProjectVersionTableComponent;
  let fixture: ComponentFixture<ProjectVersionTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjectVersionTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProjectVersionTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
