import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../components/sidebar/sidebar.component';
import { ProjectVersionTableComponent } from '../features/dashboard/project-version-table/project-version-table.component';
import { CustomCheckboxChipComponent } from '../components/custom-checkbox-chip/custom-checkbox-chip.component';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { FavoritesTabComponent } from '../features/dashboard/favorites-tab/favorites-tab.component';
import { MatButton } from '@angular/material/button';

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
    MatIcon,
    MatButton,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {}
