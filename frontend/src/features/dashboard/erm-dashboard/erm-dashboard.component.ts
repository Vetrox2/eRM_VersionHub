import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatButton } from '@angular/material/button';
import { AsyncPipe, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { SidebarComponent } from '../../../components/sidebar/sidebar.component';
import { ProjectVersionTableComponent } from '../../project-version-table/project-version-table.component';
import { AppService } from '../../../services/app.service';
import { AuthService } from '../../../services/auth.service';
import { AdminService } from '../../../services/admin.service';

@Component({
  selector: 'app-erm-dashboard',
  standalone: true,
  imports: [
    MatIconModule,
    ProjectVersionTableComponent,
    SidebarComponent,
    MatMenuModule,
    MatButton,
    AsyncPipe,
    NgIf,
    RouterLink,
  ],
  templateUrl: './erm-dashboard.component.html',
  styleUrl: './erm-dashboard.component.scss',
})
export class ErmDashboardPageComponent implements OnInit, OnDestroy {
  isSidebarActive = false;
  private newItemSelectedSubscription: Subscription | undefined;
  private appService = inject(AppService);
  private authService = inject(AuthService);
  private adminService = inject(AdminService);

  isAdmin$ = this.adminService.isAdmin$;

  ngOnInit() {
    this.newItemSelectedSubscription = this.appService.selectedApp$.subscribe(
      (selectedApp) => {
        if (selectedApp) {
          this.isSidebarActive = false;
        }
      }
    );

    this.adminService.checkAdmin();
    this.appService.loadApps().subscribe();
  }

  handleLogout() {
    this.authService.logout();
  }

  ngOnDestroy() {
    if (this.newItemSelectedSubscription) {
      this.newItemSelectedSubscription.unsubscribe();
    }
  }

  toggleSidebar() {
    this.isSidebarActive = !this.isSidebarActive;
  }
}
