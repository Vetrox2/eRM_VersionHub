import {
  ComponentFixture,
  TestBed,
  fakeAsync,
  tick,
} from '@angular/core/testing';
import { ProjectVersionTableComponent } from './project-version-table.component';
import { AppService } from '../../../services/app.service';
import { PublicationService } from '../../../services/publication.service';
import { SearchService } from '../../../services/search-service.service';
import { VersionUtilsService } from '../../../services/version-utils.service';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { of, throwError } from 'rxjs';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { FlattenedVersion } from '../../../models/flattened-version.model';
import { Version } from '../../../models/version.model';
import { Module } from '../../../models/module.model';
import { mockApps } from '../../../testing/mock-data';
import { ApiResponse } from '../../../models/api-response.model';

describe('ProjectVersionTableComponent', () => {
  let component: ProjectVersionTableComponent;
  let fixture: ComponentFixture<ProjectVersionTableComponent>;
  let appServiceSpy: jasmine.SpyObj<AppService>;
  let publicationServiceSpy: jasmine.SpyObj<PublicationService>;
  let searchServiceSpy: jasmine.SpyObj<SearchService>;
  let versionUtilsServiceSpy: jasmine.SpyObj<VersionUtilsService>;
  let dialogSpy: jasmine.SpyObj<MatDialog>;
  let snackBarSpy: jasmine.SpyObj<MatSnackBar>;

  const mockFlattenedVersion: FlattenedVersion = {
    Version: '1.0.0',
    Name: 'App1',
    ID: '1',
    Modules: [
      { Name: 'Module1', IsPublished: false, IsOptional: false },
      { Name: 'Module2', IsPublished: true, IsOptional: false },
    ],
    ParentApp: mockApps[0],
    Tag: '',
    isLoading: false,
    orignalID: '1',
  };

  const mockVersionDto: Version = {
    ID: '1',
    Number: '1.0.0',
    Modules: mockFlattenedVersion.Modules,
    PublishedTag: '',
    Name: 'App1',
  };

  beforeEach(async () => {
    const appServiceSpyObj = jasmine.createSpyObj('AppService', [
      'selectedApp$',
    ]);
    const publicationServiceSpyObj = jasmine.createSpyObj(
      'PublicationService',
      ['publishVersion', 'unPublishVersion']
    );
    const searchServiceSpyObj = jasmine.createSpyObj('SearchService', [
      'searchText$',
    ]);
    const versionUtilsServiceSpyObj = jasmine.createSpyObj(
      'VersionUtilsService',
      [
        'flattenData',
        'getPublicationStatus',
        'getPublicationStatusText',
        'flattenedVersionToDto',
      ]
    );
    const dialogSpyObj = jasmine.createSpyObj('MatDialog', ['open']);
    const snackBarSpyObj = jasmine.createSpyObj('MatSnackBar', ['open']);

    await TestBed.configureTestingModule({
      imports: [ProjectVersionTableComponent, NoopAnimationsModule],
      providers: [
        { provide: AppService, useValue: appServiceSpyObj },
        { provide: PublicationService, useValue: publicationServiceSpyObj },
        { provide: SearchService, useValue: searchServiceSpyObj },
        { provide: VersionUtilsService, useValue: versionUtilsServiceSpyObj },
        { provide: MatDialog, useValue: dialogSpyObj },
        { provide: MatSnackBar, useValue: snackBarSpyObj },
      ],
    }).compileComponents();

    appServiceSpy = TestBed.inject(AppService) as jasmine.SpyObj<AppService>;
    publicationServiceSpy = TestBed.inject(
      PublicationService
    ) as jasmine.SpyObj<PublicationService>;
    searchServiceSpy = TestBed.inject(
      SearchService
    ) as jasmine.SpyObj<SearchService>;
    versionUtilsServiceSpy = TestBed.inject(
      VersionUtilsService
    ) as jasmine.SpyObj<VersionUtilsService>;
    dialogSpy = TestBed.inject(MatDialog) as jasmine.SpyObj<MatDialog>;
    snackBarSpy = TestBed.inject(MatSnackBar) as jasmine.SpyObj<MatSnackBar>;
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProjectVersionTableComponent);
    component = fixture.componentInstance;
    appServiceSpy.selectedApp$ = of(mockApps[0]);
    searchServiceSpy.searchText$ = of('');
    versionUtilsServiceSpy.flattenData.and.returnValue([mockFlattenedVersion]);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Initialization', () => {
    it('should initialize with flattened data', () => {
      expect(component.dataSource.data).toEqual([mockFlattenedVersion]);
    });

    it('should handle null selected app', () => {
      appServiceSpy.selectedApp$ = of(null);
      component.ngOnInit();
      expect(component.dataSource.data).toEqual([]);
    });
  });

  describe('Search functionality', () => {
    it('should apply filter when search text changes', () => {
      const searchText = 'test';
      searchServiceSpy.searchText$ = of(searchText);
      component.ngOnInit();
      expect(component.dataSource.filter).toBe(searchText);
    });
  });

  describe('Row expansion', () => {
    it('should toggle row expansion', () => {
      component.toggleRow(mockFlattenedVersion, new Event('click'));
      expect(component.expandedElement).toBe(mockFlattenedVersion);
      component.toggleRow(mockFlattenedVersion, new Event('click'));
      expect(component.expandedElement).toBeNull();
    });
  });

  describe('Module checkbox handling', () => {
    it('should handle module checkbox change', () => {
      const module = mockFlattenedVersion.Modules[0];
      component.onModuleCheckboxChange(mockFlattenedVersion, module, true);
      expect(
        component.moduleChanges[module.Name + mockFlattenedVersion.orignalID]
      ).toBe(true);
    });

    it('should select all modules', () => {
      component.selectAllModules(mockFlattenedVersion);
      mockFlattenedVersion.Modules.forEach((module) => {
        expect(
          component.moduleChanges[module.Name + mockFlattenedVersion.orignalID]
        ).toBe(true);
      });
    });

    it('should unselect all modules', () => {
      component.unselectAllModules(mockFlattenedVersion);
      mockFlattenedVersion.Modules.forEach((module) => {
        expect(
          component.moduleChanges[module.Name + mockFlattenedVersion.orignalID]
        ).toBe(false);
      });
    });
  });

  describe('Publication status', () => {
    it('should get publication status', () => {
      versionUtilsServiceSpy.getPublicationStatus.and.returnValue('published');
      const status = component.getPublicationStatus(mockFlattenedVersion);
      expect(status).toBe('published');
    });

    it('should get publication status text', () => {
      versionUtilsServiceSpy.getPublicationStatusText.and.returnValue(
        'Published'
      );
      const statusText =
        component.getPublicationStatusText(mockFlattenedVersion);
      expect(statusText).toBe('Published');
    });
  });

  describe('Apply changes', () => {
    it('should open dialog when applying changes', fakeAsync(() => {
      const dialogRefSpyObj = jasmine.createSpyObj({
        afterClosed: of({ selectedTag: 'preview' }),
        close: null,
      });
      dialogSpy.open.and.returnValue(dialogRefSpyObj);

      component.moduleChanges[
        mockFlattenedVersion.Modules[0].Name + mockFlattenedVersion.orignalID
      ] = true;
      component.moduleChanges[
        mockFlattenedVersion.Modules[1].Name + mockFlattenedVersion.orignalID
      ] = false;

      publicationServiceSpy.publishVersion.and.returnValue(
        of({ Success: true, Data: true, Errors: [] } as ApiResponse<boolean>)
      );
      publicationServiceSpy.unPublishVersion.and.returnValue(
        of({ Success: true, Data: true, Errors: [] } as ApiResponse<boolean>)
      );

      component.applyChanges(mockFlattenedVersion);
      tick();

      expect(dialogSpy.open).toHaveBeenCalled();
      expect(publicationServiceSpy.publishVersion).toHaveBeenCalled();
      expect(publicationServiceSpy.unPublishVersion).toHaveBeenCalled();
    }));
    it('should handle errors when applying changes', fakeAsync(() => {
      const dialogRefSpyObj = jasmine.createSpyObj({
        afterClosed: of({ selectedTag: 'preview' }),
        close: null,
      });
      dialogSpy.open.and.returnValue(dialogRefSpyObj);

      component.moduleChanges[
        mockFlattenedVersion.Modules[0].Name + mockFlattenedVersion.orignalID
      ] = true;
      component.moduleChanges[
        mockFlattenedVersion.Modules[1].Name + mockFlattenedVersion.orignalID
      ] = false;

      const errorMessage = 'Publish error';
      publicationServiceSpy.publishVersion.and.returnValue(
        of({
          Success: false,
          Data: null,
          Errors: [errorMessage],
        } as ApiResponse<any>)
      );
      publicationServiceSpy.unPublishVersion.and.returnValue(
        of({
          Success: false,
          Data: null,
          Errors: ['Unpublish error'],
        } as ApiResponse<any>)
      );

      component.applyChanges(mockFlattenedVersion);
      tick();

      expect(snackBarSpy.open).toHaveBeenCalled();
    }));
  });

  describe('Unpublish version', () => {
    it('should handle unpublish version', fakeAsync(() => {
      const dialogRefSpyObj = jasmine.createSpyObj({
        afterClosed: of(true),
        close: null,
      });
      dialogSpy.open.and.returnValue(dialogRefSpyObj);

      publicationServiceSpy.unPublishVersion.and.returnValue(
        of({ Success: true, Data: true, Errors: [] })
      );

      component.handleUnPublishVersion(mockFlattenedVersion);
      tick();

      expect(dialogSpy.open).toHaveBeenCalled();
      expect(publicationServiceSpy.unPublishVersion).toHaveBeenCalled();
      expect(snackBarSpy.open).toHaveBeenCalledWith(
        'Version unpublished successfully',
        'Close',
        { duration: 3000 }
      );
    }));

    it('should handle errors when unpublishing version', fakeAsync(() => {
      const dialogRefSpyObj = jasmine.createSpyObj({
        afterClosed: of(true),
        close: null,
      });
      dialogSpy.open.and.returnValue(dialogRefSpyObj);

      publicationServiceSpy.unPublishVersion.and.returnValue(
        throwError(() => new Error('Unpublish error'))
      );

      component.handleUnPublishVersion(mockFlattenedVersion);
      tick();

      expect(snackBarSpy.open).toHaveBeenCalledWith(
        'Error unpublishing version',
        'Close',
        { duration: 5000 }
      );
    }));
  });
});
