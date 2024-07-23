import { TestBed } from '@angular/core/testing';

import { SearchServicesService } from './search-services.service';

describe('SearchServicesService', () => {
  let service: SearchServicesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SearchServicesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
