import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { SearchService } from './search-service.service';

describe('SearchService', () => {
  let service: SearchService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SearchService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should set and emit search text', fakeAsync(() => {
    const testText = 'test search';
    let emissionCount = 0;
    let lastEmittedValue = '';

    service.searchText$.subscribe((text) => {
      emissionCount++;
      lastEmittedValue = text;
    });

    // Clear the initial empty string emission
    tick();
    expect(emissionCount).toBe(1);
    expect(lastEmittedValue).toBe('');

    service.setSearchText(testText);
    tick();

    expect(emissionCount).toBe(2);
    expect(lastEmittedValue).toBe(testText);
  }));

  it('should emit empty string initially', fakeAsync(() => {
    let emittedValue = 'test';

    service.searchText$.subscribe((text) => {
      emittedValue = text;
    });

    tick();
    console.log('esca', emittedValue);
    expect(emittedValue).toBe('');
  }));
});
