import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { App } from '../models/app.model';

interface ApiResponse<T> {
  success: boolean;
  data: T;
  errors: string[];
}
@Injectable({
  providedIn: 'root',
})
export class AppService {
  private apiAppsUrl = 'https://localhost:7125/Apps';
  private apiFavoriteUrl = 'https://localhost:7125/Favorite';

  private appsSubject = new BehaviorSubject<App[]>([]);
  private selectedAppSubject = new BehaviorSubject<App | null>(null);
  private favoriteAppsSubject = new BehaviorSubject<App[]>([]);

  apps$ = this.appsSubject.asObservable();
  selectedApp$ = this.selectedAppSubject.asObservable();
  favoriteApps$ = this.favoriteAppsSubject.asObservable();

  constructor(private http: HttpClient) {}

  loadApps(): void {
    console.log('AppService: Loading apps...');
    this.fetchApps().subscribe({
      next: (apps) => {
        console.log('AppService: Apps loaded successfully', apps);
        this.appsSubject.next(apps);
        this.updateFavorites();
      },
      error: (error) =>
        console.error('AppService: Error fetching apps:', error),
    });
  }

  private fetchApps(): Observable<App[]> {
    return this.http.get<any>(`${this.apiAppsUrl}/admin`).pipe(
      map((response) => {
        if (response && response.Data && Array.isArray(response.Data)) {
          return response.Data as App[];
        } else {
          console.error('AppService: Unexpected API response structure');
          return [];
        }
      }),
      catchError((error) => {
        console.error('AppService: Error fetching apps:', error);
        return [];
      })
    );
  }

  getApps(): Observable<App[]> {
    return this.apps$;
  }

  setSelectedApp(app: App | null): void {
    this.selectedAppSubject.next(app);
  }

  getSelectedApp(): Observable<App | null> {
    return this.selectedApp$;
  }

  getFavoriteApps(): Observable<App[]> {
    return this.favoriteApps$;
  }

  toggleFavorite(app: App, userName: string): void {
    if (app.IsFavourite) {
      this.removeFromFavorites(userName, app.ID).subscribe({
        next: () => {
          app.IsFavourite = false;
          this.updateFavorites();
        },
        error: (error) =>
          console.error('Error removing from favorites:', error),
      });
    } else {
      this.addToFavorites(userName, app.ID).subscribe({
        next: () => {
          app.IsFavourite = true;
          this.updateFavorites();
        },
        error: (error) => console.error('Error adding to favorites:', error),
      });
    }
  }

  addToFavorite(app: App, userName: string): void {
    this.addToFavorites(userName, app.ID).subscribe({
      next: () => {
        app.IsFavourite = true;
        this.updateFavorites();
      },
      error: (error) => console.error('Error adding to favorites:', error),
    });
  }

  removeFromFavorite(app: App, userName: string): void {
    this.removeFromFavorites(userName, app.ID).subscribe({
      next: () => {
        app.IsFavourite = false;
        this.updateFavorites();
      },
      error: (error) => console.error('Error removing from favorites:', error),
    });
  }

  private updateFavorites(): void {
    const allApps = this.appsSubject.value;
    const favoriteApps = allApps.filter((app) => app.IsFavourite);
    this.favoriteAppsSubject.next(favoriteApps);
  }

  addToFavorites(userName: string, appId: string): Observable<any> {
    return this.http
      .post(`${this.apiFavoriteUrl}/${userName}/${appId}`, {})
      .pipe(
        tap(() => console.log('App added to favorites')),
        catchError((error) => {
          console.error('Error adding app to favorites:', error);
          throw error;
        })
      );
  }

  removeFromFavorites(userName: string, appId: string): Observable<any> {
    return this.http.delete(`${this.apiFavoriteUrl}/${userName}/${appId}`).pipe(
      tap(() => console.log('App removed from favorites')),
      catchError((error) => {
        console.error('Error removing app from favorites:', error);
        throw error;
      })
    );
  }
}
