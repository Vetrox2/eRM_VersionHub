import { Component } from '@angular/core';
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
    SearchComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {}
