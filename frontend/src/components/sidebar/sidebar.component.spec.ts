import {
  ComponentFixture,
  discardPeriodicTasks,
  fakeAsync,
  TestBed,
  tick,
} from '@angular/core/testing';
import { SidebarComponent } from './sidebar.component';
import { AppService } from '../../services/app.service';
import { FavoriteService } from '../../services/favorite.service';
import { BehaviorSubject, of } from 'rxjs';
import { App } from '../../models/app.model';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { mockApps } from '../../testing/mock-data';

describe('SidebarComponent', () => {
  let component: SidebarComponent;
  let fixture: ComponentFixture<SidebarComponent>;
  let appServiceMock: jasmine.SpyObj<AppService>;
  let favoriteServiceMock: jasmine.SpyObj<FavoriteService>;

  beforeEach(async () => {
    appServiceMock = jasmine.createSpyObj('AppService', ['setSelectedApp'], {
      apps$: of(mockApps),
      selectedApp$: new BehaviorSubject<App | null>(null),
    });

    favoriteServiceMock = jasmine.createSpyObj(
      'FavoriteService',
      ['addToFavorite', 'removeFromFavorite'],
      { favoriteApps$: of(mockApps.filter((app) => app.IsFavourite)) }
    );

    await TestBed.configureTestingModule({
      imports: [SidebarComponent, NoopAnimationsModule],
      providers: [
        { provide: AppService, useValue: appServiceMock },
        { provide: FavoriteService, useValue: favoriteServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(SidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should filter apps based on search term', fakeAsync(() => {
    component.onSearchValueChanged('app1');
    tick(300);
    component.apps$.subscribe((apps) => {
      expect(apps.length).toBe(1);
      expect(apps[0].Name).toBe('app1');
    });
    tick();
    discardPeriodicTasks();
  }));
  it('should change apps category', () => {
    component.setAppsCategory('Favorites');
    expect(component.currentTab).toBe('Favorites');
  });

  it('should be on favorite tab on load', fakeAsync(() => {
    component.ngOnInit();

    tick(300);

    fixture.detectChanges();

    expect(component.currentTab).toBe('Favorites');

    discardPeriodicTasks();
  }));

  it('should select an item', () => {
    const app = mockApps[0];
    component.selectItem(app);
    expect(appServiceMock.setSelectedApp).toHaveBeenCalledWith(app);
  });

  it('should add to favorites', () => {
    const app = mockApps[1];
    const event = new Event('click');
    spyOn(event, 'stopPropagation');
    component.handleFavoriteSelection(event, app);
    expect(event.stopPropagation).toHaveBeenCalled();
    expect(favoriteServiceMock.addToFavorite).toHaveBeenCalled();
  });

  it('should remove from favorites', () => {
    const app = mockApps[0];
    const event = new Event('click');
    spyOn(event, 'stopPropagation');
    component.handleFavoriteSelection(event, app);
    expect(event.stopPropagation).toHaveBeenCalled();
    expect(favoriteServiceMock.removeFromFavorite).toHaveBeenCalled();
  });

  it('should add to favorites when app is not a favorite', fakeAsync(() => {
    const app = { ...mockApps[0], IsFavourite: false };
    const event = new Event('click');
    spyOn(event, 'stopPropagation');
    component.handleFavoriteSelection(event, app);
    tick();
    expect(event.stopPropagation).toHaveBeenCalled();
    expect(favoriteServiceMock.addToFavorite).toHaveBeenCalledWith(
      app,
      'admin'
    );
    expect(favoriteServiceMock.removeFromFavorite).not.toHaveBeenCalled();
  }));

  it('should remove from favorites when app is already a favorite', fakeAsync(() => {
    const app = { ...mockApps[0], IsFavourite: true };
    const event = new Event('click');
    spyOn(event, 'stopPropagation');
    component.handleFavoriteSelection(event, app);
    tick();
    expect(event.stopPropagation).toHaveBeenCalled();
    expect(favoriteServiceMock.removeFromFavorite).toHaveBeenCalledWith(
      app,
      'admin'
    );
    expect(favoriteServiceMock.addToFavorite).not.toHaveBeenCalled();
  }));
});
