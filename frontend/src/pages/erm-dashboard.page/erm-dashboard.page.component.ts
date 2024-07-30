import { Component, inject } from '@angular/core';
import { Subscription } from 'rxjs';
import { AppService } from '../../services/app.service';
import { MatIconModule } from '@angular/material/icon';
import { ProjectVersionTableComponent } from '../../features/dashboard/project-version-table/project-version-table.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { MatMenuModule } from '@angular/material/menu';
import { MatButton } from '@angular/material/button';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-erm-dashboard.page',
  standalone: true,
  imports: [
    MatIconModule,
    ProjectVersionTableComponent,
    SidebarComponent,
    MatMenuModule,
    MatButton,
  ],
  templateUrl: './erm-dashboard.page.component.html',
  styleUrl: './erm-dashboard.page.component.scss',
})
export class ErmDashboardPageComponent {
  isSidebarActive = false;
  private newItemSelectedSubscription: Subscription | undefined;
  private appService = inject(AppService);
  private authService = inject(AuthService);

  ngOnInit() {
    this.newItemSelectedSubscription = this.appService.selectedApp$.subscribe(
      (selectedApp) => {
        if (selectedApp) {
          this.isSidebarActive = false;
        }
      }
    );
    this.appService.loadApps().subscribe(
      (apps) => console.log('Apps loaded in component:', apps),
      (error) => console.error('Error loading apps in component:', error),
      () => console.log('loadApps completed in component')
    );
  }
  hanldeLogout() {
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
