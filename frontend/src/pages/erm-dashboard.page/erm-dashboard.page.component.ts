import { Component, inject } from '@angular/core';
import { Subscription } from 'rxjs';
import { AppService } from '../../services/app.service';
import { MatIconModule } from '@angular/material/icon';
import { ProjectVersionTableComponent } from '../../features/dashboard/project-version-table/project-version-table.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';

@Component({
  selector: 'app-erm-dashboard.page',
  standalone: true,
  imports: [MatIconModule, ProjectVersionTableComponent, SidebarComponent],
  templateUrl: './erm-dashboard.page.component.html',
  styleUrl: './erm-dashboard.page.component.scss',
})
export class ErmDashboardPageComponent {
  isSidebarActive = false;
  private newItemSelectedSubscription: Subscription | undefined;
  private appService = inject(AppService);

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

  ngOnDestroy() {
    if (this.newItemSelectedSubscription) {
      this.newItemSelectedSubscription.unsubscribe();
    }
  }

  toggleSidebar() {
    this.isSidebarActive = !this.isSidebarActive;
  }
}
