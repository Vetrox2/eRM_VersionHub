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
    <p style="padding-left: 5px">Favorites:</p>
    <ng-container *ngIf="favoriteApps$ | async as favoriteApps">
      <mat-chip-set>
        <mat-chip *ngFor="let app of favoriteApps" style="margin: 5px">
          <div
            style="display: flex; align-items: center; justify-content: space-between; width: 100%;"
          >
            <span>{{ app.Name }}</span>
            <app-menu-icons
              [menuItems]="menuItems"
              (menuSelection)="handleMenuSelection($event, app)"
            ></app-menu-icons>
          </div>
        </mat-chip>
      </mat-chip-set>
    </ng-container>
  `,
  styles: [
    `
      mat-chip-set {
        display: flex;
        flex-wrap: wrap;
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
}
