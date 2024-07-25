import { Component, OnInit } from '@angular/core';
import { MatChip, MatChipSet, MatChipsModule } from '@angular/material/chips';
import { NgFor, AsyncPipe, CommonModule } from '@angular/common';
import { App } from '../../../models/app.model';
import { AppService } from '../../../services/app-service.service';
import { Observable } from 'rxjs';
import { MatIcon } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import {
  MenuIconsComponent,
  MenuItem,
} from '../../../components/menu/menu.component';

@Component({
  selector: 'app-favorites-tab',
  standalone: true,
  imports: [
    NgFor,
    AsyncPipe,
    MatChip,
    MatChipSet,
    MatIcon,
    CommonModule,
    MatMenuModule,
    MatButtonModule,
    MenuIconsComponent,
    MatChipsModule,
  ],
  template: `
    <p>Favorites:</p>

    <div class="chip-container">
      <ng-container *ngIf="favoriteApps$ | async as favoriteApps">
        <div class="tag-selector-scroll">
          <mat-chip-set class="chip-set">
            <mat-chip
              [style.background-color]="'var(--primary-color)'"
              *ngFor="let app of favoriteApps"
              (click)="onChipClick(app)"
            >
              <div [style.color]="'white'" class="chip-content">
                <span>{{ app.Name }}</span>
                <app-menu-icons
                  [menuItems]="menuItems"
                  (menuSelection)="handleMenuSelection($event, app)"
                ></app-menu-icons>
              </div>
            </mat-chip>
          </mat-chip-set>
        </div>
      </ng-container>
    </div>
  `,
  styles: [
    `
      .chip-container {
        width: 100%;
        overflow-x: auto;
        padding: 5px 0;
      }
      .chip-set {
        display: flex;
        flex-wrap: nowrap;
      }
      ::ng-deep .mdc-evolution-chip-set__chips {
        display: flex !important;
        flex-wrap: nowrap !important;
      }
      .tag-selector-scroll {
        overflow-x: auto;
        white-space: nowrap;
        scrollbar-width: none; /* Firefox */
        -ms-overflow-style: none; /* Internet Explorer 10+ */
      }
      .tag-selector-scroll::-webkit-scrollbar {
        width: 0;
        height: 0;
        display: none; /* Safari and Chrome */
      }
      mat-chip {
        flex: 0 0 auto;
        margin-right: 10px;
        cursor: pointer;
      }
      .chip-content {
        display: flex;
        align-items: center;
        justify-content: space-between;
        width: 100%;
      }
      app-menu-icons {
        margin-left: 8px;
      }
    `,
  ],
})
export class FavoritesTabComponent implements OnInit {
  favoriteApps$: Observable<App[]>;

  menuItems: MenuItem[] = [
    {
      icon: 'favorite_border',
      label: 'Remove from favorites',
      action: 'remove_favorite',
    },
  ];

  constructor(private appService: AppService) {
    this.favoriteApps$ = this.appService.getFavoriteApps();
  }

  ngOnInit() {
    this.appService.loadApps();
  }

  handleMenuSelection(action: string, app: App) {
    if (action === 'remove_favorite') {
      this.appService.removeFromFavorite(app, 'admin');
    }
  }

  onChipClick(app: App) {
    this.appService.setSelectedApp(app);
  }
}
