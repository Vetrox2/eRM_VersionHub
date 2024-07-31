import { Component, OnInit } from '@angular/core';
import { Observable, combineLatest, map, of, switchMap } from 'rxjs';
import { AdminService } from '../../services/admin.service';
import { UserSelectionService } from '../../services/user-selection.service';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTooltipModule } from '@angular/material/tooltip';
import { UserApps } from '../../models/user-apps.model';
import { ApiResponse } from '../../models/api-response.model';
import { SearchComponent } from '../../components/search/search.component';
import { App } from '../../models/app.model';
import { Module } from '../../models/module.model';

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
    MatExpansionModule,
    MatTooltipModule,
    SearchComponent,
  ],
  templateUrl: './user-app-permissions.component.html',
  styleUrls: ['./user-app-permissions.component.scss'],
})
export class UserAppPermissionsComponent implements OnInit {
  selectedUser$: Observable<UserApps | null>;
  allApps$: Observable<string[]> = of([]);
  detailedApps$: Observable<App[]> = of([]);
  filteredApps$: Observable<string[]> = of([]);
  appPermissions$: Observable<{ [key: string]: boolean }> = of({});

  constructor(
    private adminService: AdminService,
    private userSelectionService: UserSelectionService
  ) {
    this.selectedUser$ = this.userSelectionService.getSelectedUser();
  }

  ngOnInit() {
    this.loadApps();
  }

  loadApps() {
    this.allApps$ = this.adminService
      .getAllApps()
      .pipe(map((response: ApiResponse<string[]>) => response.Data));

    this.detailedApps$ = this.adminService.getAllAppsDetailed();

    this.filteredApps$ = this.allApps$;

    this.appPermissions$ = combineLatest([
      this.allApps$,
      this.selectedUser$,
    ]).pipe(
      map(([allApps, user]) => {
        const permissions: { [key: string]: boolean } = {};
        allApps.forEach((appName) => {
          permissions[appName] = user
            ? user.AppsNames.includes(appName)
            : false;
        });
        return permissions;
      })
    );
  }

  // getModules(appName: string): Observable<Module[]> {
  //   return this.detailedApps$.pipe(
  //     map((apps) => {
  //       const app = apps.find((a) => a.Name === appName);
  //       if (!app) return [];
  //       return this.getUniqueModules(app);
  //     })
  //   );
  // }

  // getUniqueModules(app: App): Module[] {
  //   const moduleMap = new Map<string, Module>();
  //   app.Versions.forEach((version) => {
  //     version.Modules.forEach((module) => {
  //       if (!moduleMap.has(module.Name)) {
  //         moduleMap.set(module.Name, module);
  //       }
  //     });
  //   });
  //   return Array.from(moduleMap.values());
  // }

  addAllPermissions(username: string) {
    combineLatest([this.allApps$, this.appPermissions$])
      .pipe(
        map(([allApps, permissions]) =>
          allApps.filter((app) => !permissions[app])
        )
      )
      .subscribe((appsToAdd) => {
        appsToAdd.forEach((app) => this.addPermission(username, app));
      });
  }

  removeAllPermissions(username: string) {
    combineLatest([this.allApps$, this.appPermissions$])
      .pipe(
        map(([allApps, permissions]) =>
          allApps.filter((app) => permissions[app])
        )
      )
      .subscribe((appsToRemove) => {
        appsToRemove.forEach((app) => this.removePermission(username, app));
      });
  }

  filterApps(searchTerm: string) {
    this.filteredApps$ = this.allApps$.pipe(
      map((apps) =>
        apps.filter((app) =>
          app.toLowerCase().includes(searchTerm.toLowerCase())
        )
      )
    );
  }

  togglePermission(username: string, appName: string, hasPermission: boolean) {
    if (hasPermission) {
      this.removePermission(username, appName);
    } else {
      this.addPermission(username, appName);
    }
  }

  private addPermission(username: string, appName: string) {
    this.adminService.addPermission(username, appName).subscribe(() => {
      this.refreshUserApps(username);
    });
  }

  private removePermission(username: string, appName: string) {
    this.adminService.removePermission(username, appName).subscribe(() => {
      this.refreshUserApps(username);
    });
  }

  private refreshUserApps(username: string) {
    this.adminService
      .getUsersWithApps()
      .subscribe((response: ApiResponse<UserApps[]>) => {
        const updatedUser = response.Data.find(
          (user) => user.Username === username
        );
        if (updatedUser) {
          this.userSelectionService.setSelectedUser(updatedUser);
        }
      });
  }
}
