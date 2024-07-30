import {
  Component,
  OnInit,
  OnDestroy,
  ViewChild,
  AfterViewInit,
  ChangeDetectorRef,
  Input,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSort, MatSortModule, Sort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import {
  trigger,
  state,
  style,
  transition,
  animate,
} from '@angular/animations';
import {
  catchError,
  finalize,
  forkJoin,
  lastValueFrom,
  Observable,
  Subscription,
  tap,
} from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

import { App } from '../../../models/app.model';
import { Tag, Version } from '../../../models/version.model';
import { Module } from '../../../models/module.model';
import { AppService } from '../../../services/app.service';
import { DefaultModalComponent } from '../../../components/modals/default-modal/default-modal.component';
import { SearchService } from '../../../services/search-service.service';
import { SearchComponent } from '../../../components/search/search.component';
import { FlattenedVersion } from '../../../models/flattened-version.model';
import { VersionUtilsService } from '../../../services/version-utils.service';
import { MatChip } from '@angular/material/chips';
import { PublicationService } from '../../../services/publication.service';
import { ApiResponse } from '../../../models/api-response.model';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-project-version-table',
  standalone: true,
  templateUrl: './project-version-table.component.html',
  styleUrls: ['./project-version-table.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition(
        'expanded <=> collapsed',
        animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')
      ),
    ]),
  ],
  imports: [
    CommonModule,
    MatTableModule,
    MatCheckboxModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatSortModule,
    SearchComponent,
    MatChip,
  ],
})
export class ProjectVersionTableComponent
  implements OnInit, OnDestroy, AfterViewInit
{
  @ViewChild(MatSort) sort!: MatSort;
  dataSource: MatTableDataSource<FlattenedVersion>;
  moduleChanges: { [key: string]: boolean } = {};
  isLoading: boolean = false;
  selectedTag: Tag = '';
  arrowStates: { [key: string]: string } = {};
  @Input() searchTerm: string = '';
  columnsToDisplay = ['Version', 'Tag', 'Published', 'expand'];
  expandedElement: FlattenedVersion | null = null;
  private selectedAppSubscription: Subscription | undefined;
  private searchSubscription: Subscription | undefined;
  selectedAppName: string = '';
  constructor(
    private appService: AppService,
    private publicationService: PublicationService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private searchService: SearchService,
    private versionUtils: VersionUtilsService
  ) {
    this.dataSource = new MatTableDataSource<FlattenedVersion>([]);
  }

  ngOnInit() {
    this.selectedAppSubscription = this.appService.selectedApp$.subscribe(
      (selectedApp) => {
        if (selectedApp) {
          this.flattenData([selectedApp]);
          this.selectedAppName = selectedApp.Name;
        } else {
          this.dataSource.data = [];
        }
      }
    );

    // Subscribe to search text changes
    this.searchSubscription = this.searchService.searchText$.subscribe(
      (searchText) => {
        this.applyFilter(searchText); // Apply filter based on search text
      }
    );
  }
  applyFilter(searchText: string) {
    if (this.dataSource) {
      this.dataSource.filter = searchText.trim().toLowerCase();
    }
  }
  toggleRow(version: FlattenedVersion, event: Event) {
    this.expandedElement = this.expandedElement === version ? null : version;
    event?.stopPropagation();
  }
  ngAfterViewInit() {
    if (this.dataSource) {
      this.dataSource.sort = this.sort;
      this.dataSource.sortingDataAccessor = (
        item: FlattenedVersion,
        property: string
      ) => {
        return (item as any)[property];
      };
    }

    // Set custom filter predicate
    this.dataSource.filterPredicate = (
      data: FlattenedVersion,
      filter: string
    ): boolean => {
      if (!filter) {
        return true; // Show all data when filter is empty
      }
      const filterLowerCase = filter.toLowerCase();
      return (
        data.Version.toLowerCase().includes(filterLowerCase) ||
        (data.Tag?.toLowerCase().includes(filterLowerCase) ?? false) ||
        (data.Name?.toLowerCase().includes(filterLowerCase) ?? false)
      );
    };
  }

  ngOnDestroy() {
    if (this.selectedAppSubscription) {
      this.selectedAppSubscription.unsubscribe();
    }
  }
  onModuleCheckboxChange(
    version: FlattenedVersion,
    module: Module,
    isChecked: boolean
  ) {
    this.moduleChanges[module.Name + version.orignalID] = isChecked;
    this.dataSource.data = [...this.dataSource.data];
  }

  applyChanges(flattenedVersion: FlattenedVersion) {
    const updatedModules = flattenedVersion.Modules.map((module) => ({
      ...module,
      IsPublished:
        this.moduleChanges[module.Name + flattenedVersion.orignalID] ??
        module.IsPublished,
    }));

    const publishedModules = updatedModules.filter((mod) => mod.IsPublished);
    const unpublishedModules = updatedModules.filter((mod) => !mod.IsPublished);

    if (publishedModules.length === 0 && unpublishedModules.length > 0) {
      this.handleUnPublishVersion(flattenedVersion);
      return;
    }

    const dialogRef = this.dialog.open(DefaultModalComponent, {
      data: {
        title: 'Confirm Changes',
        message: `Are you sure you want to apply changes to module(s)?`,
        showTags: true,
        selectedTag: flattenedVersion.Tag || 'none',
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result && result.selectedTag) {
        this.isLoading = true;
        flattenedVersion.isLoading = true;

        const versionDtoPublish = {
          ...this.flattenedVersionToDto(flattenedVersion),
          Modules: publishedModules,
          PublishedTag: result.selectedTag === 'none' ? '' : result.selectedTag,
        };

        const versionDtoUnPublish = {
          ...this.flattenedVersionToDto(flattenedVersion),
          Modules: unpublishedModules,
        };

        const observables: Observable<ApiResponse<any>>[] = [];

        if (publishedModules.length > 0) {
          observables.push(
            this.publicationService.publishVersion(versionDtoPublish)
          );
        }

        if (unpublishedModules.length > 0) {
          observables.push(
            this.publicationService.unPublishVersion(versionDtoUnPublish)
          );
        }

        let publishError = false;
        let unpublishError = false;
        let failedPublishModules: string[] = [];
        let failedUnpublishModules: string[] = [];
        let completedCount = 0;

        observables.forEach((observable, index) => {
          observable.subscribe({
            next: (response: ApiResponse<boolean>) => {
              if (!response.Success) {
                if (index === 0) {
                  publishError = true;
                  failedPublishModules = publishedModules.map((m) => m.Name);
                } else {
                  unpublishError = true;
                  failedUnpublishModules = unpublishedModules.map(
                    (m) => m.Name
                  );
                }
              }
              completedCount++;
              if (completedCount === observables.length) {
                this.handleCompletion(
                  flattenedVersion,
                  result,
                  publishError,
                  unpublishError,
                  failedPublishModules,
                  failedUnpublishModules
                );
              }
            },
          });
        });
      }
    });
  }
  private handleCompletion(
    flattenedVersion: FlattenedVersion,
    result: any,
    publishError: boolean,
    unpublishError: boolean,
    failedPublishModules: string[],
    failedUnpublishModules: string[]
  ) {
    this.isLoading = false;
    flattenedVersion.isLoading = false;

    if (publishError || unpublishError) {
      let errorMessage = '';
      if (publishError && unpublishError) {
        errorMessage = `Failed to publish: ${failedPublishModules.join(
          ', '
        )}. Failed to unpublish: ${failedUnpublishModules.join(', ')}.`;
      } else if (publishError) {
        errorMessage = `Failed to publish: ${failedPublishModules.join(', ')}.`;
      } else if (unpublishError) {
        errorMessage = `Failed to unpublish: ${failedUnpublishModules.join(
          ', '
        )}.`;
      }

      this.snackBar.open(errorMessage, 'Close', { duration: 5000 });
    } else {
      flattenedVersion.Modules.forEach((module) => {
        if (
          this.moduleChanges[module.Name + flattenedVersion.orignalID] !==
          undefined
        ) {
          module.IsPublished =
            this.moduleChanges[module.Name + flattenedVersion.orignalID];
        }
      });
      flattenedVersion.Tag = result.selectedTag;

      this.snackBar.open('Changes applied successfully', 'Close', {
        duration: 3000,
      });
    }

    this.moduleChanges = {}; // Clear the changes
    this.refreshData();
  }

  private handleError(error: any, errorMessage: string): void {
    console.error('Error:', error);
    let displayMessage = errorMessage;
    if (error.error && error.error.message) {
      displayMessage += ': ' + error.error.message;
    }
    this.snackBar.open(displayMessage, 'Close', { duration: 5000 });
  }

  flattenData(apps: App[]) {
    const flattenedData = this.versionUtils.flattenData(apps);
    this.dataSource = new MatTableDataSource<FlattenedVersion>(flattenedData);
    if (this.sort) {
      this.dataSource.sort = this.sort;
    }
  }

  getPublicationStatus(version: FlattenedVersion) {
    return this.versionUtils.getPublicationStatus(version);
  }
  getPublicationStatusText(version: FlattenedVersion) {
    return this.versionUtils.getPublicationStatusText(version);
  }

  flattenedVersionToDto(flattenedVersion: FlattenedVersion): Version {
    return this.versionUtils.flattenedVersionToDto(flattenedVersion);
  }

  handleUnPublishVersion(flattenedVersion: FlattenedVersion) {
    const dialogRef = this.dialog.open(DefaultModalComponent, {
      data: {
        title: 'Unpublish Version',
        message: 'Are you sure you want to unpublish this version?',
        showTags: false,
      },
    });
    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        flattenedVersion.isLoading = true;
        const versionDto = this.flattenedVersionToDto(flattenedVersion);

        this.publicationService.unPublishVersion(versionDto).subscribe({
          next: (response) => {
            if (response.Success) {
              flattenedVersion.Tag = '';
              this.snackBar.open('Version unpublished successfully', 'Close', {
                duration: 3000,
              });
              flattenedVersion.Modules.forEach((module) => {
                module.IsPublished = false;
              });
              flattenedVersion.isLoading = false;
              this.refreshData();
            } else {
              flattenedVersion.isLoading = false;
              this.handleError(
                'Error unpublishing version',
                response.Errors.join(', ')
              );
              this.refreshData();
              return;
            }
          },
          error: (error) => {
            this.handleError('publish error', 'Error unpublishing version');
          },
        });
      }
    });
  }
  getPublishedVersionsCount(): number {
    return this.dataSource.data.filter(
      (v) => this.getPublicationStatus(v) === 'published'
    ).length;
  }

  selectAllModules(version: FlattenedVersion) {
    version.Modules.forEach((module) => {
      this.moduleChanges[module.Name + version.orignalID] = true;
    });
    this.dataSource.data = [...this.dataSource.data];
  }

  unselectAllModules(version: FlattenedVersion) {
    version.Modules.forEach((module) => {
      this.moduleChanges[module.Name + version.orignalID] = false;
    });
    this.dataSource.data = [...this.dataSource.data];
  }

  isModuleSelected(version: FlattenedVersion, module: Module): boolean {
    return (
      this.moduleChanges[module.Name + version.orignalID] ?? module.IsPublished
    );
  }
  areAllModulesSelected(element: any): boolean {
    return element.Modules.every((module: any) =>
      this.isModuleSelected(element, module)
    );
  }

  private refreshData() {
    if (this.dataSource) {
      this.dataSource.data = [...this.dataSource.data];
    }
  }
}
