import { Component, OnDestroy, OnInit } from '@angular/core';
import { BehaviorSubject, Observable, Subject, combineLatest, of } from 'rxjs';
import {
  map,
  takeUntil,
  switchMap,
  distinctUntilChanged,
  catchError,
  filter,
  finalize,
} from 'rxjs/operators';
import { AdminService } from '../../services/admin.service';
import { UserSelectionService } from '../../services/user-selection.service';
import { AsyncPipe, KeyValuePipe, NgFor, NgIf } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SearchComponent } from '../../components/search/search.component';
import { AppPermission } from '../../models/app-permissions.model';

@Component({
  selector: 'app-user-app-permissions',
  standalone: true,
  imports: [
    AsyncPipe,
    NgFor,
    NgIf,
    MatListModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    SearchComponent,
    KeyValuePipe,
    MatProgressSpinnerModule,
  ],
  templateUrl: './user-app-permissions.component.html',
  styleUrls: ['./user-app-permissions.component.scss'],
})
export class UserAppPermissionsComponent implements OnDestroy, OnInit {
  selectedUser$: Observable<string>;
  permissions$: Observable<AppPermission | null>;
  filteredApps$!: Observable<Record<string, boolean> | null>;

  private permissionsSubject = new BehaviorSubject<AppPermission | null>(null);
  private searchTerm$ = new BehaviorSubject<string>('');
  private destroy$ = new Subject<void>();
  isLoading = false;

  constructor(
    private adminService: AdminService,
    private userSelectionService: UserSelectionService,
    private snackBar: MatSnackBar
  ) {
    this.selectedUser$ =
      this.userSelectionService.selectedUserSubject.asObservable();
    this.permissions$ = this.permissionsSubject.asObservable();
  }
  loadingApps: Set<string> = new Set();

  ngOnInit() {
    this.selectedUser$.subscribe((user) => {
      if (user) {
        this.isLoading = false;
        this.initializePermissions();
        this.initializeFilteredApps();
      }
    });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializePermissions() {
    this.isLoading = true;
    this.selectedUser$
      .pipe(
        filter((user) => !!user && user.length > 0),
        switchMap((user) => this.adminService.getAppPermission(user)),
        catchError((error) => {
          this.showErrorSnackbar('Error fetching permissions');
          return of(null);
        }),
        finalize(() => (this.isLoading = false)),
        takeUntil(this.destroy$)
      )
      .subscribe((permissions) => {
        this.permissionsSubject.next(permissions);
        this.isLoading = false;
      });
  }

  private initializeFilteredApps() {
    this.filteredApps$ = combineLatest([
      this.permissions$,
      this.searchTerm$.pipe(distinctUntilChanged()),
    ]).pipe(
      map(([permissions, searchTerm]) =>
        this.filterAppsBySearchTerm(permissions, searchTerm)
      ),
      takeUntil(this.destroy$)
    );
  }

  private filterAppsBySearchTerm(
    permissions: AppPermission | null,
    searchTerm: string
  ): Record<string, boolean> | null {
    if (!permissions) return null;
    return Object.entries(permissions.AppsPermission)
      .filter(([appName]) =>
        appName.toLowerCase().includes(searchTerm.toLowerCase())
      )
      .reduce((acc, [key, value]) => ({ ...acc, [key]: value }), {});
  }

  filterApps(searchTerm: string) {
    this.searchTerm$.next(searchTerm);
  }

  togglePermission(username: string, appName: string, hasPermission: boolean) {
    this.loadingApps.add(appName);
    const action = hasPermission
      ? this.adminService.addPermission(username, appName)
      : this.adminService.removePermission(username, appName);

    action
      .pipe(
        catchError((error) => {
          console.error('Error in togglePermission:', error);
          this.showErrorSnackbar(
            'Error toggling permission. Please try again.'
          );
          // Return an observable to continue the chain
          return of(null);
        }),
        takeUntil(this.destroy$),
        finalize(() => {
          this.loadingApps.delete(appName);
          console.log('Finalized');
        })
      )
      .subscribe({
        next: (result) => {
          if (result !== null) {
            this.updateLocalPermissionsState(appName, hasPermission);
            this.showSuccessSnackbar('Permission updated successfully');
          } else {
            // Revert the local state if there was an error
            this.updateLocalPermissionsState(appName, !hasPermission);
          }
        },
        complete: () => {
          console.log('Completed');
        },
      });
  }

  private showSuccessSnackbar(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
    });
  }

  private showErrorSnackbar(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
    });
  }

  isAppLoading(appName: string): boolean {
    return this.loadingApps.has(appName);
  }

  private updateLocalPermissionsState(appName: string, hasPermission: boolean) {
    const currentPermissions = this.permissionsSubject.getValue();
    if (currentPermissions) {
      const updatedPermissions = {
        ...currentPermissions,
        AppsPermission: {
          ...currentPermissions.AppsPermission,
          [appName]: hasPermission,
        },
      };
      this.permissionsSubject.next(updatedPermissions);
      this.loadingApps.delete(appName);
    }
  }

  getAppCount(user: AppPermission): number {
    return user?.AppsPermission ? Object.keys(user.AppsPermission).length : 0;
  }

  getPermittedAppCount(user: AppPermission): number {
    return user?.AppsPermission
      ? Object.values(user.AppsPermission).filter(Boolean).length
      : 0;
  }
}
