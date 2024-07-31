import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SearchComponent } from './search.component';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { SearchService } from '../../services/search-service.service';

describe('SearchComponent', () => {
  let component: SearchComponent;
  let fixture: ComponentFixture<SearchComponent>;
  let searchService: jasmine.SpyObj<SearchService>;

  beforeEach(async () => {
    const searchServiceSpy = jasmine.createSpyObj('SearchService', [
      'someMethod',
    ]);

    await TestBed.configureTestingModule({
      imports: [
        FormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatIconModule,
        MatButtonModule,
        NoopAnimationsModule,
        SearchComponent,
      ],
      providers: [{ provide: SearchService, useValue: searchServiceSpy }],
    }).compileComponents();

    searchService = TestBed.inject(
      SearchService
    ) as jasmine.SpyObj<SearchService>;
    fixture = TestBed.createComponent(SearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default values', () => {
    expect(component.showIcons).toBeTrue();
    expect(component.placeholder).toBe('Search');
    expect(component.value).toBe('');
  });

  it('should emit value on input change', () => {
    spyOn(component.valueChanged, 'emit');
    component.value = 'test';
    component.onInputChange();
    expect(component.valueChanged.emit).toHaveBeenCalledWith('test');
  });

  it('should clear search', () => {
    spyOn(component.valueChanged, 'emit');
    component.value = 'test';
    component.clearSearch();
    expect(component.value).toBe('');
    expect(component.valueChanged.emit).toHaveBeenCalledWith('');
  });

  it('should show search icon when showIcons is true', () => {
    component.showIcons = true;
    fixture.detectChanges();
    const searchIcon = fixture.nativeElement.querySelector(
      'mat-icon[matIconPrefix]'
    );
    expect(searchIcon).toBeTruthy();
    expect(searchIcon.textContent.trim()).toBe('search');
  });

  it('should not show search icon when showIcons is false', () => {
    component.showIcons = false;
    fixture.detectChanges();
    const searchIcon = fixture.nativeElement.querySelector(
      'mat-icon[matIconPrefix]'
    );
    expect(searchIcon).toBeFalsy();
  });

  it('should show clear button when showIcons is true and value is not empty', () => {
    component.showIcons = true;
    component.value = 'test';
    fixture.detectChanges();
    const clearButton = fixture.nativeElement.querySelector(
      'button[aria-label="Clear"]'
    );
    expect(clearButton).toBeTruthy();
  });

  it('should not show clear button when showIcons is false', () => {
    component.showIcons = false;
    component.value = 'test';
    fixture.detectChanges();
    const clearButton = fixture.nativeElement.querySelector(
      'button[aria-label="Clear"]'
    );
    expect(clearButton).toBeFalsy();
  });

  it('should not show clear button when value is empty', () => {
    component.showIcons = true;
    component.value = '';
    fixture.detectChanges();
    const clearButton = fixture.nativeElement.querySelector(
      'button[aria-label="Clear"]'
    );
    expect(clearButton).toBeFalsy();
  });
});
