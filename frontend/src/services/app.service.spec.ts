import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AppService } from './app.service';
import { ApiService } from './api.service';
import { of, throwError } from 'rxjs';
import { App } from '../models/app.model';
import { ApiResponse } from '../models/api-response.model';

describe('AppService', () => {
  let service: AppService;
  let apiServiceSpy: jasmine.SpyObj<ApiService>;

  const mockApps: App[] = [
    { ID: '1', Name: 'App 1', Versions: [], IsFavourite: false },
    { ID: '2', Name: 'App 2', Versions: [], IsFavourite: true },
  ];

  beforeEach(() => {
    const spy = jasmine.createSpyObj('ApiService', ['get']);

    TestBed.configureTestingModule({
      providers: [AppService, { provide: ApiService, useValue: spy }],
    });

    service = TestBed.inject(AppService);
    apiServiceSpy = TestBed.inject(ApiService) as jasmine.SpyObj<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('setSelectedTag', () => {
    it('should update the selected tag', fakeAsync(() => {
      const testTag = 'test-tag';
      service.setSelectedTag(testTag);

      service.selectedTag$.subscribe((tag) => {
        expect(tag).toBe(testTag);
      });
      tick();
    }));
  });

  describe('loadApps', () => {
    it('should load apps successfully', fakeAsync(() => {
      const mockResponse: ApiResponse<App[]> = {
        Success: true,
        Data: mockApps,
        Errors: [],
      };
      apiServiceSpy.get.and.returnValue(of(mockResponse));

      let loadedApps: App[] | undefined;
      service.loadApps().subscribe((apps) => {
        loadedApps = apps;
      });
      tick();

      expect(loadedApps).toEqual(mockApps);
      expect(apiServiceSpy.get).toHaveBeenCalledWith('Apps/admin');

      service.apps$.subscribe((apps) => {
        expect(apps).toEqual(mockApps);
      });
      tick();
    }));

    it('should handle error when loading apps', fakeAsync(() => {
      apiServiceSpy.get.and.returnValue(
        throwError(() => new Error('Test error'))
      );

      let loadedApps: App[] | undefined;
      service.loadApps().subscribe((apps) => {
        loadedApps = apps;
      });
      tick();

      expect(loadedApps).toEqual([]);
      expect(apiServiceSpy.get).toHaveBeenCalledWith('Apps/admin');
    }));
  });

  describe('setSelectedApp', () => {
    it('should update the selected app', fakeAsync(() => {
      const testApp = mockApps[0];
      service.setSelectedApp(testApp);

      service.selectedApp$.subscribe((app) => {
        expect(app).toEqual(testApp);
      });
      tick();
    }));

    it('should set selected app to null', fakeAsync(() => {
      service.setSelectedApp(null);

      service.selectedApp$.subscribe((app) => {
        expect(app).toBeNull();
      });
      tick();
    }));
  });

  describe('updateApp', () => {
    it('should update an existing app', fakeAsync(() => {
      const updatedApp: App = { ...mockApps[0], Name: 'Updated App 1' };

      service['appsSubject'].next(mockApps);
      service.updateApp(updatedApp);

      service.apps$.subscribe((apps) => {
        const app = apps.find((a) => a.ID === updatedApp.ID);
        expect(app).toEqual(updatedApp);
        expect(apps.length).toBe(mockApps.length);
      });
      tick();
    }));

    it('should not add a new app if ID does not exist', fakeAsync(() => {
      const newApp: App = {
        ID: '3',
        Name: 'New App',
        Versions: [],
        IsFavourite: false,
      };

      service['appsSubject'].next(mockApps);
      service.updateApp(newApp);

      service.apps$.subscribe((apps) => {
        expect(apps).toEqual(mockApps);
        expect(apps.length).toBe(mockApps.length);
      });
      tick();
    }));
  });
});
