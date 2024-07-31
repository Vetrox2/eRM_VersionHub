import { Component, inject } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { UserSidebarComponent } from '../../../components/sidebar/user-sidebar/user-sidebar/user-sidebar.component';
import { UserAppPermissionsComponent } from '../../user-app-permissions/user-app-permissions.component';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    MatIconModule,
    MatMenuModule,
    UserSidebarComponent,
    UserAppPermissionsComponent,
    MatButtonModule,
    RouterLink,
  ],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss',
})
export class AdminDashboardComponent {
  private authService = inject(AuthService);

  toggleSidebar() {
    this.isSidebarActive = !this.isSidebarActive;
  }
  handleLogout() {
    this.authService.logout();
  }
  isSidebarActive = false;
}
