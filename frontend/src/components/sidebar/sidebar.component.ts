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
import { AppService } from '../../services/app.service';
import {
  debounceTime,
  distinctUntilChanged,
  filter,
  map,
  take,
} from 'rxjs/operators';
import { MenuIconsComponent, MenuItem } from '../menu/menu.component';
import { ToggleAppSelectorComponent } from '../toggle-app-selector/toggle-app-selector.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FavoriteService } from '../../services/favorite.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
  imports: [
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule,
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
    ToggleAppSelectorComponent,
  ],
})
export class SidebarComponent implements OnInit {
  private searchTerm$ = new BehaviorSubject<string>('');
  private currentTabAppsCategory$ = new BehaviorSubject<string>('All');
  private firstRun = true;
  isFiltering = false;
  favoriteApps$: Observable<App[]>;
  apps$: Observable<App[]>;
  selectedItem$: Observable<App | null>;
  loading = true;
  error: string | null = null;
  currentTab: 'All' | 'Favorites' = 'All';

  constructor(
    private appService: AppService,
    private favoriteService: FavoriteService
  ) {
    this.favoriteApps$ = favoriteService.favoriteApps$;
    this.favoriteService.favoriteApps$.subscribe((apps) => console.log(apps));
    this.apps$ = combineLatest([
      this.appService.apps$,
      this.favoriteApps$,
      this.currentTabAppsCategory$,
      this.searchTerm$.pipe(debounceTime(300), distinctUntilChanged()),
    ]).pipe(
      map(([apps, favoriteApps, currentTab, searchTerm]) => {
        this.loading = false;
        this.isFiltering = !!searchTerm;
        const filteredApps = this.filterApps(
          apps,
          favoriteApps,
          currentTab,
          searchTerm
        );
        return filteredApps;
      })
    );

    this.selectedItem$ = this.appService.selectedApp$;
  }

  setAppsCategory(appsCategory: 'All' | 'Favorites') {
    this.currentTab = appsCategory;
    this.currentTabAppsCategory$.next(appsCategory);
  }
  private filterApps(
    apps: App[],
    favoriteApps: App[],
    isFavorites: string,
    searchTerm: string
  ): App[] {
    let filteredApps =
      isFavorites === 'All'
        ? apps
        : isFavorites === 'Favorites'
        ? favoriteApps
        : apps;

    if (searchTerm) {
      filteredApps = filteredApps.filter((app) =>
        app.Name.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    return filteredApps;
  }

  onSearchValueChanged(value: string) {
    this.searchTerm$.next(value);
  }

  ngOnInit() {
    this.loadApps();
  }

  private loadApps() {
    this.loading = true;
    this.appService.apps$.subscribe({
      next: () => {
        this.loading = false;
        this.initializeSelectedApp();
      },
      error: (error) => {
        this.loading = false;
        this.error = 'Failed to load apps. Please try again.';
        console.error('Error loading apps:', error);
      },
    });
  }

  private initializeSelectedApp() {
    combineLatest([this.apps$])
      .pipe(
        filter(([apps]) => apps.length > 0),
        take(1)
      )
      .subscribe(([apps]) => {
        const favoriteApps = apps.filter((app) => app.IsFavourite);
        if (this.firstRun) {
          if (favoriteApps.length > 0) {
            this.appService.setSelectedApp(favoriteApps[0]);
            this.setAppsCategory('Favorites');
          } else {
            this.appService.setSelectedApp(apps[0]);
            this.setAppsCategory('All');
          }
          this.firstRun = false;
        }
      });
  }

  selectItem(item: App) {
    this.appService.setSelectedApp(item);
  }

  trackById(index: number, app: App): string {
    return app.ID;
  }

  handleFavoriteSelection(event: Event, app: App) {
    event.stopPropagation();
    if (app.IsFavourite) {
      this.favoriteService.removeFromFavorite(app, 'admin');
    } else {
      this.favoriteService.addToFavorite(app, 'admin');
    }
  }
  getFavoriteIcon(app: App): string {
    return app.IsFavourite ? 'favorite' : 'favorite_border';
  }
}
