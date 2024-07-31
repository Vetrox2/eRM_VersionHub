import { TestBed } from '@angular/core/testing';

import { UserSelectionService } from './user-selection.service';

describe('UserSelectionService', () => {
  let service: UserSelectionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserSelectionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
