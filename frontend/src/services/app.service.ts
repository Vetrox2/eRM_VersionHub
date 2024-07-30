import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { App } from '../models/app.model';
import { Version } from '../models/version.model';
import { ApiService } from './api.service';
import { FavoriteService } from './favorite.service';
import { ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root',
})
export class AppService {
  private appsSubject = new BehaviorSubject<App[]>([]);
  private selectedAppSubject = new BehaviorSubject<App | null>(null);
  private tagSubject = new BehaviorSubject<string>('');

  apps$ = this.appsSubject.asObservable();
  selectedApp$ = this.selectedAppSubject.asObservable();
  selectedTag$ = this.tagSubject.asObservable();

  constructor(private apiService: ApiService) {}

  setSelectedTag(tag: string): void {
    this.tagSubject.next(tag);
  }
  loadApps(): Observable<App[]> {
    console.log('loading');
    return this.apiService.get<ApiResponse<App[]>>('Apps').pipe(
      map((response) => {
        const apps = response.Data || [];
        this.appsSubject.next(apps);
        return apps;
      }),
      catchError((error) => {
        console.error('Error fetching apps:', error);
        return of([]);
      })
    );
  }
  setSelectedApp(app: App | null): void {
    this.selectedAppSubject.next(app);
  }
  updateApp(updatedApp: App): void {
    const currentApps = this.appsSubject.value;
    const updatedApps = currentApps.map((app) =>
      app.ID === updatedApp.ID ? updatedApp : app
    );
    this.appsSubject.next(updatedApps);
  }
}
