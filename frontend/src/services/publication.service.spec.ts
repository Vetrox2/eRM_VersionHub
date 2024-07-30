import { TestBed } from '@angular/core/testing';
import { PublicationService } from './publication.service';
import { ApiService } from './api.service';
import { of } from 'rxjs';
import { Version } from '../models/version.model';
import { ApiResponse } from '../models/api-response.model';

describe('PublicationService', () => {
  let service: PublicationService;
  let apiServiceSpy: jasmine.SpyObj<ApiService>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj('ApiService', ['post', 'delete']);

    TestBed.configureTestingModule({
      providers: [PublicationService, { provide: ApiService, useValue: spy }],
    });

    service = TestBed.inject(PublicationService);
    apiServiceSpy = TestBed.inject(ApiService) as jasmine.SpyObj<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should publish version', (done) => {
    const mockVersion: Version = {
      ID: '1',
      Number: '1.0.0',
      Modules: [],
      Name: 'Test',
      PublishedTag: '',
    };
    const mockResponse: ApiResponse<any> = {
      Success: true,
      Data: {},
      Errors: [],
    };

    apiServiceSpy.post.and.returnValue(of(mockResponse));

    service.publishVersion(mockVersion).subscribe((response) => {
      expect(response).toEqual(mockResponse);
      expect(apiServiceSpy.post).toHaveBeenCalledWith(
        'Publication',
        mockVersion
      );
      done();
    });
  });

  it('should unpublish version', (done) => {
    const mockVersion: Version = {
      ID: '1',
      Number: '1.0.0',
      Modules: [],
      Name: 'Test',
      PublishedTag: '',
    };
    const mockResponse: ApiResponse<any> = {
      Success: true,
      Data: {},
      Errors: [],
    };

    apiServiceSpy.delete.and.returnValue(of(mockResponse));

    service.unPublishVersion(mockVersion).subscribe((response) => {
      expect(response).toEqual(mockResponse);
      expect(apiServiceSpy.delete).toHaveBeenCalledWith(
        'Publication',
        mockVersion
      );
      done();
    });
  });
});
