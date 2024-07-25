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
import { ToggleAppSelectorComponent } from '../toggle-app-selector/toggle-app-selector.component';

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
    ToggleAppSelectorComponent
  ],
})
export class SidebarComponent implements OnInit {
  private searchTerm$ = new BehaviorSubject<string>('');
  private isFavorites$ = new BehaviorSubject<string>('All');
  isFiltering = false;
  favoriteApps$: Observable<App[]>;
  apps$: Observable<App[]>;
  selectedItem$: Observable<App | null>;
  loading = true;
  error: string | null = null;

  constructor(private appService: AppService) {
    const apps$ = this.appService.getApps().pipe(
      catchError((err) => {
        this.loading = false;
        this.error = 'Failed to load apps. Please try again later.';
        console.error('Error loading apps:', err);
        return of([]);
      })
    );

    this.favoriteApps$ = this.appService.getFavoriteApps();

    this.apps$ = combineLatest([
      apps$,
      this.favoriteApps$,
      this.isFavorites$,
      this.searchTerm$.pipe(debounceTime(300), distinctUntilChanged()),
    ]).pipe(
      map(([apps, favoriteApps, isFavorites, searchTerm]) => {
        this.loading = false;
        this.isFiltering = !!searchTerm;
        
        const filteredApps = this.filterApps(apps, favoriteApps, isFavorites, searchTerm);
        
        if (isFavorites !== 'All' && filteredApps.length === 0) {
          return [];
        }
        
        return filteredApps;
      })
    );
  
    this.selectedItem$ = this.appService.getSelectedApp();
  }

  onActive(active: string) {
    this.isFavorites$.next(active);
    console.log(this.appService.getFavoriteApps()  )
  }

  private filterApps(apps: App[], favoriteApps: App[], isFavorites: string, searchTerm: string): App[] {
    let filteredApps = isFavorites === 'All'
      ? apps
      : isFavorites === 'Favorites'
        ? favoriteApps
        : apps;

    if (searchTerm) {
      filteredApps = filteredApps.filter(app => 
        app.Name.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    return filteredApps;
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

  favoriteClickCount: { [key: string]: number } = {};

  handleFavoriteSelection(event: Event, app: App) {
    event.stopPropagation();
    
    if (app.IsFavourite) {
      this.appService.removeFromFavorite(app, 'admin');
    } else {
      this.appService.addToFavorite(app, 'admin');
    }

    // The service will update the favorites list internally
  }

  getFavoriteIcon(app: App): string {
    return app.IsFavourite ? 'favorite' : 'favorite_border';
  }
}