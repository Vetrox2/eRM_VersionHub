import { TestBed } from '@angular/core/testing';

import { VersionUtilsService } from './version-utils.service';

describe('VersionUtilsService', () => {
  let service: VersionUtilsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(VersionUtilsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
