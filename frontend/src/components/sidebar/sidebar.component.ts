import { Component, OnInit } from '@angular/core';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { AsyncPipe, NgClass, NgFor, NgIf } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { SearchComponent } from '../search/search.component';
import { SelectionToggleComponent } from '../selection-toggle/selection-toggle/selection-toggle.component';
import { App } from '../../models/app.model';
import { BehaviorSubject, combineLatest, Observable, of } from 'rxjs';
import { AppService } from '../../services/app-service.service';
import {
  catchError,
  debounceTime,
  distinctUntilChanged,
  map,
  switchMap,
} from 'rxjs/operators';
import { MenuIconsComponent, MenuItem } from '../menu/menu.component';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
  imports: [
    MatListModule,
    MatIconModule,
    MatButtonModule,
    NgClass,
    NgFor,
    NgIf,
    AsyncPipe,
    MatInputModule,
    MatFormFieldModule,
    SearchComponent,
    SelectionToggleComponent,
    MenuIconsComponent,
    SearchComponent,
  ],
})
export class SidebarComponent implements OnInit {
  private searchTerm$ = new BehaviorSubject<string>('');
  isFiltering = false;

  apps$: Observable<App[]>;
  selectedItem$: Observable<App | null>;
  loading = true;
  error: string | null = null;

  menuItems: MenuItem[] = [
    { icon: 'publish', label: 'Publish', action: 'publish' },
    { icon: 'cancel', label: 'Unpublish', action: 'unpublish' },
    { icon: 'favorite', label: 'Add to favorites', action: 'favorite' },
  ];

  constructor(private appService: AppService) {
    this.apps$ = combineLatest([
      this.appService.getApps(),
      this.searchTerm$.pipe(debounceTime(300), distinctUntilChanged()),
    ]).pipe(
      switchMap(([apps, searchTerm]) => {
        this.loading = false;
        this.isFiltering = !!searchTerm;
        return of(this.filterApps(apps, searchTerm));
      }),
      catchError((err) => {
        this.loading = false;
        this.error = 'Failed to load apps. Please try again later.';
        console.error('Error loading apps:', err);
        return of([]);
      })
    );

    this.selectedItem$ = this.appService.getSelectedApp();
  }

  private filterApps(apps: App[], searchTerm: string): App[] {
    if (!searchTerm) {
      return apps;
    }
    searchTerm = searchTerm.toLowerCase();
    return apps.filter((app) => app.Name.toLowerCase().includes(searchTerm));
  }
  onSearchValueChanged(value: string) {
    this.searchTerm$.next(value);
  }
  ngOnInit() {
    this.appService.loadApps();
  }

  selectItem(item: App) {
    this.appService.setSelectedApp(item);
  }

  trackById(index: number, app: App): string {
    return app.ID;
  }

  handleMenuSelection(action: string, app: App) {
    console.log(`Action ${action} selected for app ${app.Name}`);
    switch (action) {
      case 'publish':
        // Handle publish action
        break;
      case 'unpublish':
        // Handle unpublish action
        break;
      case 'favorite':
        this.appService.addToFavorite(app, 'admin');
        break;
      default:
        console.warn(`Unknown action: ${action}`);
    }
  }
}
