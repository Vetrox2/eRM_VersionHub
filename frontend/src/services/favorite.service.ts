import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
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

  addToFavorite(app: App, userName: string): void {
    this.addToFavoritesApiCall(userName, app.ID).subscribe({
      next: () => {
        app.IsFavourite = true;
        this.appService.updateApp(app);
      },
      error: (error: any) => console.error('Error adding to favorites:', error),
    });
  }

  removeFromFavorite(app: App, userName: string): void {
    this.removeFromFavoritesApiCall(userName, app.ID).subscribe({
      next: () => {
        app.IsFavourite = false;
        this.appService.updateApp(app);
      },
      error: (error: any) =>
        console.error('Error removing from favorites:', error),
    });
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
