import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { FavoriteService } from './favorite.service';
import { ApiService } from './api.service';
import { AppService } from './app.service';
import { of, throwError, BehaviorSubject } from 'rxjs';
import { App } from '../models/app.model';

describe('FavoriteService', () => {
  let service: FavoriteService;
  let apiServiceSpy: jasmine.SpyObj<ApiService>;
  let appServiceSpy: jasmine.SpyObj<AppService>;

  const mockApps: App[] = [
    { ID: '1', Name: 'App 1', Versions: [], IsFavourite: false },
    { ID: '2', Name: 'App 2', Versions: [], IsFavourite: true },
  ];

  beforeEach(() => {
    const apiSpy = jasmine.createSpyObj('ApiService', ['post', 'delete']);
    const appSpy = jasmine.createSpyObj('AppService', ['updateApp'], {
      apps$: new BehaviorSubject<App[]>(mockApps),
    });

    TestBed.configureTestingModule({
      providers: [
        FavoriteService,
        { provide: ApiService, useValue: apiSpy },
        { provide: AppService, useValue: appSpy },
      ],
    });

    service = TestBed.inject(FavoriteService);
    apiServiceSpy = TestBed.inject(ApiService) as jasmine.SpyObj<ApiService>;
    appServiceSpy = TestBed.inject(AppService) as jasmine.SpyObj<AppService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('addToFavorite', () => {
    it('should add an app to favorites', fakeAsync(() => {
      const app = mockApps[0];
      const userName = 'testUser';

      apiServiceSpy.post.and.returnValue(of({}));

      service.addToFavorite(app, userName).subscribe();

      tick();

      expect(apiServiceSpy.post).toHaveBeenCalledWith(
        `Favorite/${userName}/${app.ID}`,
        {}
      );
      expect(app.IsFavourite).toBe(true);
      expect(appServiceSpy.updateApp).toHaveBeenCalledWith(app);
    }));

    it('should handle error when adding to favorites', fakeAsync(() => {
      const app = mockApps[0];
      const userName = 'testUser';

      const testError = new Error('Test error');
      apiServiceSpy.post.and.returnValue(throwError(() => testError));
      spyOn(console, 'error');

      service.addToFavorite(app, userName).subscribe(
        () => fail('should have failed'),
        (error) => {
          expect(error).toBe(testError);
        }
      );

      tick();

      expect(apiServiceSpy.post).toHaveBeenCalledWith(
        `Favorite/${userName}/${app.ID}`,
        {}
      );
      expect(console.error).toHaveBeenCalledWith(
        'Error adding to favorites:',
        testError
      );
      expect(appServiceSpy.updateApp).not.toHaveBeenCalled();
    }));
  });

  describe('removeFromFavorite', () => {
    it('should remove an app from favorites', fakeAsync(() => {
      const app = mockApps[1];
      const userName = 'testUser';

      apiServiceSpy.delete.and.returnValue(of({}));

      service.removeFromFavorite(app, userName).subscribe();

      tick();

      expect(apiServiceSpy.delete).toHaveBeenCalledWith(
        `Favorite/${userName}/${app.ID}`
      );
      expect(app.IsFavourite).toBe(false);
      expect(appServiceSpy.updateApp).toHaveBeenCalledWith(app);
    }));

    it('should handle error when removing from favorites', fakeAsync(() => {
      const app = mockApps[1];
      const userName = 'testUser';

      const testError = new Error('Test error');
      apiServiceSpy.delete.and.returnValue(throwError(() => testError));
      spyOn(console, 'error');

      service.removeFromFavorite(app, userName).subscribe(
        () => fail('should have failed'),
        (error) => {
          expect(error).toBe(testError);
        }
      );

      tick();

      expect(apiServiceSpy.delete).toHaveBeenCalledWith(
        `Favorite/${userName}/${app.ID}`
      );
      expect(console.error).toHaveBeenCalledWith(
        'Error removing from favorites:',
        testError
      );
      expect(appServiceSpy.updateApp).not.toHaveBeenCalled();
    }));
  });

  it('should update favorites when apps change', fakeAsync(() => {
    let favoriteApps: App[] = [];
    service.favoriteApps$.subscribe((apps) => {
      favoriteApps = apps;
    });

    const newApps = [
      { ID: '1', Name: 'App 1', Versions: [], IsFavourite: true },
      { ID: '2', Name: 'App 2', Versions: [], IsFavourite: false },
      { ID: '3', Name: 'App 3', Versions: [], IsFavourite: true },
    ];

    (appServiceSpy.apps$ as BehaviorSubject<App[]>).next(newApps);

    tick();

    expect(favoriteApps.length).toBe(2);
    expect(favoriteApps[0]).toEqual(newApps[0]);
    expect(favoriteApps[1]).toEqual(newApps[2]);
  }));
});
