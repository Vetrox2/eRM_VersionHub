import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { AppService } from '../services/app.service';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../components/sidebar/sidebar.component';
import { ProjectVersionTableComponent } from '../features/dashboard/project-version-table/project-version-table.component';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
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
    MatChipsModule,
    MatDividerModule,
    MatIconModule,
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
    this.newItemSelectedSubscription = this.appService.selectedApp$.subscribe(
      (selectedApp) => {
        if (selectedApp) {
          this.isSidebarActive = false;
        }
      }
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
