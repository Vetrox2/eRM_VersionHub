import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { App } from '../models/app.model';
import { ApiService } from './api.service';
import { AppService } from './app.service';

@Injectable({
  providedIn: 'root',
})
export class FavoriteService {
  private favoriteAppsSubject = new BehaviorSubject<App[]>([]);
  favoriteApps$ = this.favoriteAppsSubject.asObservable();

  constructor(private apiService: ApiService, private appService: AppService) {
    this.appService.apps$.subscribe(this.updateFavorites.bind(this));
  }

  private updateFavorites(apps: App[]): void {
    const favoriteApps = apps.filter((app) => app.IsFavourite);
    this.favoriteAppsSubject.next(favoriteApps);
  }

  addToFavorite(app: App, userName: string): Observable<void> {
    return this.addToFavoritesApiCall(userName, app.ID).pipe(
      map(() => {
        app.IsFavourite = true;
        this.appService.updateApp(app);
      }),
      catchError((error) => {
        console.error('Error adding to favorites:', error);
        return throwError(() => error);
      })
    );
  }

  removeFromFavorite(app: App, userName: string): Observable<void> {
    return this.removeFromFavoritesApiCall(userName, app.ID).pipe(
      map(() => {
        app.IsFavourite = false;
        this.appService.updateApp(app);
      }),
      catchError((error) => {
        console.error('Error removing from favorites:', error);
        return throwError(() => error);
      })
    );
  }

  private addToFavoritesApiCall(
    userName: string,
    appId: string
  ): Observable<any> {
    return this.apiService.post(`Favorite/${userName}/${appId}`, {});
  }

  private removeFromFavoritesApiCall(
    userName: string,
    appId: string
  ): Observable<any> {
    return this.apiService.delete(`Favorite/${userName}/${appId}`);
  }
}
