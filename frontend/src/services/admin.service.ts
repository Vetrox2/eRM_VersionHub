import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject, of, throwError } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse } from '../models/api-response.model';
import { AppService } from './app.service';
import { App } from '../models/app.model';
import { map, catchError, tap, first } from 'rxjs/operators';
import { AppPermission } from '../models/app-permissions.model';
import { response } from 'express';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private usersSubject = new BehaviorSubject<string[]>([]);
  users$ = this.usersSubject.asObservable();

  private isAdminSubject = new BehaviorSubject<boolean>(false);
  isAdmin$ = this.isAdminSubject.asObservable();

  private userPermissionsSubject = new BehaviorSubject<AppPermission | null>(
    null
  );
  userPermissions$ = this.userPermissionsSubject.asObservable();

  constructor(private apiService: ApiService, private appService: AppService) {}

  isAdmin(): Observable<ApiResponse<boolean>> {
    return this.apiService
      .get<ApiResponse<boolean>>('Admin')
      .pipe(tap((res) => this.isAdminSubject.next(res.Success)));
  }

  getUsers(): Observable<ApiResponse<string[]>> {
    return this.apiService.get<ApiResponse<string[]>>('User');
  }

  getAppPermission(username: string): Observable<AppPermission> {
    return this.apiService
      .get<ApiResponse<AppPermission>>(`Permission/${username}`)
      .pipe(
        map((response) => {
          return response.Data || [];
        })
      );
  }

  getAllAppsDetailed(): Observable<App[]> {
    return this.appService.loadApps();
  }

  checkAdmin() {
    this.isAdmin().subscribe();
  }

  loadUsers() {
    this.getUsers().subscribe(
      (response) => {
        this.usersSubject.next(response.Data);
      },
      (error) => {
        console.error('Error loading users:', error);
        this.usersSubject.next([]);
      }
    );
  }

  addPermission(username: string, appID: string): Observable<any> {
    return this.apiService.post('Permission', { username, appID }).pipe(
      catchError((error) => {
        console.error('Error adding permission:', error);
        return throwError(() => new Error('Failed to add permission'));
      })
    );
  }

  removePermission(username: string, appID: string): Observable<any> {
    return this.apiService.delete(`Permission`, { username, appID }).pipe(
      catchError((error) => {
        console.error('Error removing permission:', error);
        return throwError(() => new Error('Failed to remove permission'));
      })
    );
  }

  private updateUserPermissions(
    username: string,
    appID: string,
    hasPermission: boolean
  ) {
    const currentPermissions = this.userPermissionsSubject.getValue();
    if (currentPermissions && currentPermissions.User === username) {
      const updatedPermissions = {
        ...currentPermissions,
        AppsPermission: {
          ...currentPermissions.AppsPermission,
          [appID]: hasPermission,
        },
      };
      this.userPermissionsSubject.next(updatedPermissions);
    }
  }
}
