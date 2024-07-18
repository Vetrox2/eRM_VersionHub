import {
  Component,
  OnInit,
  OnDestroy,
  ViewChild,
  AfterViewInit,
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
import { Subscription } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

import { App } from '../../../models/app.model';
import { Version } from '../../../models/version.model';
import { Module } from '../../../models/module.model';
import { AppService } from '../../../services/app-service.service';
import { StatusChipComponent } from '../../../components/status-chip/status-chip.component';
import { MenuIconsComponent } from '../../../components/menu/menu.component';
import { ChipDropdownComponent } from '../../../components/chip-dropdown/chip-dropdown.component';
import { DefaultModalComponent } from '../../../components/modals/default-modal/default-modal.component';

interface FlattenedVersion {
  Version: string;
  Name: string;
  ID: string;
  Modules: App['Versions'][0]['Modules'];
  ParentApp: App;
  Tag: string;
  isLoading: boolean;
  orignalID: string;
}

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
    StatusChipComponent,
    MatButtonModule,
    MenuIconsComponent,
    MatIconModule,
    ChipDropdownComponent,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatSortModule,
  ],
})
export class ProjectVersionTableComponent
  implements OnInit, OnDestroy, AfterViewInit
{
  @ViewChild(MatSort) sort!: MatSort;
  dataSource: MatTableDataSource<FlattenedVersion>;

  moduleChanges: { [key: string]: boolean } = {};
  isLoading: boolean = false;

  columnsToDisplay = ['Actions', 'Published', 'Version', 'Tag'];
  expandedElement: FlattenedVersion | null = null;
  private selectedAppSubscription: Subscription | undefined;
  constructor(
    private appService: AppService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    this.dataSource = new MatTableDataSource<FlattenedVersion>([]);
  }

  ngOnInit() {
    this.selectedAppSubscription = this.appService
      .getSelectedApp()
      .subscribe((selectedApp) => {
        if (selectedApp) {
          this.flattenData([selectedApp]);
        } else {
          this.dataSource.data = [];
        }
      });
  }

  ngAfterViewInit() {
    if (this.dataSource) {
      this.dataSource.sort = this.sort;
      this.dataSource.sortingDataAccessor = (
        item: FlattenedVersion,
        property: string
      ) => {
        switch (property) {
          case 'Published':
            return this.getPublicationStatusValue(
              this.getPublicationStatus(item)
            );
          default:
            return (item as any)[property];
        }
      };
    }
  }
  getPublicationStatusValue(
    status: 'published' | 'semi-published' | 'not-published'
  ): number {
    switch (status) {
      case 'published':
        return 2;
      case 'semi-published':
        return 1;
      case 'not-published':
        return 0;
      default:
        return -1;
    }
  }

  ngOnDestroy() {
    if (this.selectedAppSubscription) {
      this.selectedAppSubscription.unsubscribe();
    }
  }

  onModuleCheckboxChange(module: Module, isChecked: boolean) {
    this.moduleChanges[module.Name] = isChecked;
  }

  applyChanges(version: FlattenedVersion) {
    const changedModules = version.Modules.filter(
      (module) =>
        this.moduleChanges[module.Name] !== undefined &&
        this.moduleChanges[module.Name] !== module.IsPublished
    );

    if (changedModules.length === 0) {
      // this.snackBar.open('No changes to apply', 'Close', { duration: 3000 });
      // return;
    }

    const dialogRef = this.dialog.open(DefaultModalComponent, {
      data: {
        title: 'Confirm Changes',
        message: `Are you sure you want to apply changes to ${changedModules.length} module(s)?`,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.isLoading = true;
        const publishModules = changedModules.filter(
          (module) => this.moduleChanges[module.Name]
        );
        const unpublishModules = changedModules.filter(
          (module) => !this.moduleChanges[module.Name]
        );

        const publishVersion: Version | null =
          publishModules.length > 0
            ? {
                ...this.flattenedVersionToDto(version),
                Modules: publishModules,
              }
            : null;

        const unpublishVersion: Version | null =
          unpublishModules.length > 0
            ? {
                ...this.flattenedVersionToDto(version),
                Modules: unpublishModules,
              }
            : null;

        const promises: Promise<any>[] = [];

        if (publishVersion) {
          promises.push(
            this.appService.publishVersion(publishVersion).toPromise()
          );
        }

        if (unpublishVersion) {
          promises.push(
            this.appService.unPublishVersion(unpublishVersion).toPromise()
          );
        }

        Promise.all(promises)
          .then(() => {
            this.snackBar.open('Changes applied successfully', 'Close', {
              duration: 3000,
            });
            version.Modules.forEach((module) => {
              if (this.moduleChanges[module.Name] !== undefined) {
                module.IsPublished = this.moduleChanges[module.Name];
              }
            });
            this.moduleChanges = {};
            this.refreshData();
          })
          .catch((error) => {
            this.snackBar.open('Error applying changes: ' + error, 'Close', {
              duration: 5000,
            });
          })
          .finally(() => {
            this.isLoading = false;
          });
      }
    });
  }

  getTagOptions(tag: string): string[] {
    return ['Scoped', 'Preview', 'None'];
  }

  onTagSelected(option: string, element: FlattenedVersion) {
    element.Tag = option;
  }

  flattenData(apps: App[]) {
    const flattenedData = apps.flatMap((app) =>
      app.Versions.map((version) => ({
        Version: version.Name,
        Tag: version.Tag,
        Name: app.Name,
        ID: app.ID,
        Modules: version.Modules,
        ParentApp: app,
        isLoading: false,
        orignalID: version.ID,
      }))
    );
    this.dataSource = new MatTableDataSource<FlattenedVersion>(flattenedData);
    if (this.sort) {
      this.dataSource.sort = this.sort;
    }
  }

  getPublicationStatus(
    version: FlattenedVersion
  ): 'published' | 'semi-published' | 'not-published' {
    const publishedCount = version.Modules.filter((m) => m.IsPublished).length;
    if (publishedCount === version.Modules.length) {
      return 'published';
    } else if (publishedCount > 0) {
      return 'semi-published';
    } else {
      return 'not-published';
    }
  }

  flattenedVersionToDto(flattenedVersion: FlattenedVersion): Version {
    type tag = 'scoped' | 'preview' | 'none' | '';
    return {
      ID: flattenedVersion.orignalID,
      Modules: flattenedVersion.Modules,
      Name: flattenedVersion.Version,
      Tag: flattenedVersion.Tag === 'none' ? '' : (flattenedVersion.Tag as tag),
    };
  }

  handlePublishVersion(flattenedVersion: FlattenedVersion) {
    this.showConfirmationDialog(
      'Publish Version',
      'Are you sure you want to publish this version?'
    ).then((confirmed) => {
      if (confirmed) {
        flattenedVersion.isLoading = true;
        const versionDto = this.flattenedVersionToDto(flattenedVersion);
        console.log(versionDto);
        this.appService.publishVersion(versionDto).subscribe({
          next: (response) => {
            if (response.success) {
              this.snackBar.open('Version published successfully', 'Close', {
                duration: 3000,
              });
              flattenedVersion.Modules.forEach((module) => {
                module.IsPublished = true;
              });
              this.refreshData();
            } else {
              this.snackBar.open(
                'Error publishing version: ' + response.errors.join(', '),
                'Close',
                { duration: 5000 }
              );
            }
          },
          error: (error) => {
            this.snackBar.open('Error publishing version: ' + error, 'Close', {
              duration: 5000,
            });
          },
          complete: () => {
            flattenedVersion.isLoading = false;
            this.refreshData();
          },
        });
      }
    });
  }

  handleUnPublishVersion(flattenedVersion: FlattenedVersion) {
    this.showConfirmationDialog(
      'Unpublish Version',
      'Are you sure you want to unpublish this version?'
    ).then((confirmed) => {
      if (confirmed) {
        flattenedVersion.isLoading = true;
        const versionDto = this.flattenedVersionToDto(flattenedVersion);

        this.appService.unPublishVersion(versionDto).subscribe({
          next: (response) => {
            if (response.success) {
              this.snackBar.open('Version unpublished successfully', 'Close', {
                duration: 3000,
              });
              flattenedVersion.Modules.forEach((module) => {
                module.IsPublished = false;
              });
              this.refreshData();
            } else {
              this.snackBar.open(
                'Error unpublishing version: ' + response.errors.join(', '),
                'Close',
                { duration: 5000 }
              );
            }
          },
          error: (error) => {
            this.snackBar.open(
              'Error unpublishing version: ' + error,
              'Close',
              { duration: 5000 }
            );
          },
          complete: () => {
            flattenedVersion.isLoading = false;
            this.refreshData();
          },
        });
      }
    });
  }

  private refreshData() {
    if (this.dataSource) {
      this.dataSource.data = [...this.dataSource.data];
    }
  }

  private showConfirmationDialog(
    title: string,
    message: string
  ): Promise<boolean> {
    const dialogRef = this.dialog.open(DefaultModalComponent, {
      data: { title, message },
    });

    return dialogRef.afterClosed().toPromise();
  }

  getPublicationIcon(version: FlattenedVersion): string {
    const status = this.getPublicationStatus(version);
    switch (status) {
      case 'published':
        return 'check_circle';
      case 'not-published':
        return 'cancel';
      default:
        return 'remove';
    }
  }

  getPublicationColor(version: FlattenedVersion): string {
    const status = this.getPublicationStatus(version);
    switch (status) {
      case 'published':
        return '#1b701e';
      case 'not-published':
        return '#f53c37';
      default:
        return 'orange';
    }
  }
}
