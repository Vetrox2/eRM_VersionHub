import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse } from '../models/api-response.model';
import { UserApps } from '../models/user-apps.model';
import { AppService } from './app.service';
import { App } from '../models/app.model';
import { map, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private usersSubject = new BehaviorSubject<UserApps[]>([]);
  users$ = this.usersSubject.asObservable();

  private isAdminSubject = new BehaviorSubject<boolean>(false);
  isAdmin$ = this.isAdminSubject.asObservable();

  constructor(private apiService: ApiService, private appService: AppService) {}

  getAllApps(): Observable<ApiResponse<string[]>> {
    return this.apiService.get<ApiResponse<string[]>>('Apps/AppsNames');
  }

  isAdmin(): Observable<ApiResponse<boolean>> {
    return this.apiService.get<ApiResponse<boolean>>('Admin');
  }

  getUsersWithApps(): Observable<ApiResponse<UserApps[]>> {
    return this.apiService.get<ApiResponse<UserApps[]>>('User/UsersWithApps');
  }

  getAllAppsDetailed(): Observable<App[]> {
    return this.appService.loadApps();
  }

  checkAdmin() {
    this.isAdmin().subscribe((res) => {
      this.isAdminSubject.next(res.Success);
    });
  }

  loadUsers() {
    this.getUsersWithApps().subscribe(
      (response: ApiResponse<UserApps[]>) => {
        this.usersSubject.next(response.Data);
      },
      (error) => {
        console.error('Error loading users:', error);
      }
    );
  }

  addPermission(username: string, appID: string): Observable<any> {
    return this.apiService.post('Permission', { username, appID });
  }

  removePermission(username: string, appID: string): Observable<any> {
    return this.apiService.delete('Permission', { username, appID });
  }
}
