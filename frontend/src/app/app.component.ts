import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { AppService } from '../services/app-service.service';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../components/sidebar/sidebar.component';
import { ProjectVersionTableComponent } from '../features/dashboard/project-version-table/project-version-table.component';
import { CustomCheckboxChipComponent } from '../components/custom-checkbox-chip/custom-checkbox-chip.component';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { FavoritesTabComponent } from '../features/dashboard/favorites-tab/favorites-tab.component';
import { MatButtonModule } from '@angular/material/button';
import { SearchComponent } from '../components/search/search.component';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatSidenavModule } from '@angular/material/sidenav';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    SidebarComponent,
    ProjectVersionTableComponent,
    CustomCheckboxChipComponent,
    MatChipsModule,
    MatDividerModule,
    MatIconModule,
    FavoritesTabComponent,
    MatButtonModule,
    MatInputModule,
    FormsModule,
    SearchComponent,
    MatSidenavModule,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit, OnDestroy {
  isSidebarActive = false;
  private newItemSelectedSubscription: Subscription | undefined;
  private appService = inject(AppService);

  ngOnInit() {
    this.newItemSelectedSubscription = this.appService
      .getSelectedApp()
      .subscribe((selectedApp) => {
        if (selectedApp) {
          this.isSidebarActive = false;
        }
      });
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
