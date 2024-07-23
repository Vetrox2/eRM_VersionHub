import { TestBed } from '@angular/core/testing';
import { AppService } from './app-service.service';

describe('AppServiceService', () => {
  let service: AppService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AppService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
