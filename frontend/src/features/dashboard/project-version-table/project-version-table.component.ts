import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import {
  trigger,
  state,
  style,
  transition,
  animate,
} from '@angular/animations';
import { App } from '../../../models/app.model';
import { StatusChipComponent } from '../../../components/status-chip/status-chip.component';
import { MatButtonModule } from '@angular/material/button';
import { AppService } from '../../../services/app-service.service';
import { Subscription } from 'rxjs';
import { MenuIconsComponent } from '../../../components/menu/menu.component';
import { Version } from '../../../models/version.model';
import { MatIcon } from '@angular/material/icon';
import { ChipDropdownComponent } from '../../../components/chip-dropdown/chip-dropdown.component';
import { MatDividerModule } from '@angular/material/divider';
import { Module } from '../../../models/module.model';
import { MatDialog } from '@angular/material/dialog';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DefaultModalComponent } from '../../../components/modals/default-modal/default-modal.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { response } from 'express';

interface FlattenedVersion {
  Version: string;
  Name: string;
  ID: string;
  Modules: App['Versions'][0]['Modules'];
  ParentApp: App;
  Tag: string;
  isLoading: boolean;
}
export interface MenuItem {
  icon: string;
  label: string;
  action: string;
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
    MatIcon,
    ChipDropdownComponent,
    MatDividerModule,
    MatProgressSpinnerModule,
  ],
})
export class ProjectVersionTableComponent implements OnInit, OnDestroy {
  dataSource: FlattenedVersion[] = [];
  moduleChanges: { [key: string]: boolean } = {};
  isLoading: boolean = false;

  columnsToDisplay = ['Actions', 'Published', 'Version', 'Tag', 'Name', 'ID'];
  expandedElement: FlattenedVersion | null = null;
  private selectedAppSubscription: Subscription | undefined;
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
      this.snackBar.open('No changes to apply', 'Close', { duration: 3000 });
      return;
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
        const promises = changedModules.map((module) => {
          const updatedVersion: Version = {
            ...this.flattenedVersionToDto(version),
            Modules: [module],
          };

          if (this.moduleChanges[module.Name]) {
            return this.appService.publishVersion(updatedVersion).toPromise();
          } else {
            return this.appService.unPublishVersion(updatedVersion).toPromise();
          }
        });

        Promise.all(promises)
          .then(() => {
            this.snackBar.open('Changes applied successfully', 'Close', {
              duration: 3000,
            });
            this.moduleChanges = {};
            // Refresh data here if needed
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

  menuItems: MenuItem[] = [
    { icon: 'publish', label: 'Publish', action: 'publish' },
    { icon: 'cancel', label: 'Unpublish', action: 'unpublish' },
  ];

  constructor(
    private appService: AppService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}
  ngOnInit() {
    this.selectedAppSubscription = this.appService
      .getSelectedApp()
      .subscribe((selectedApp) => {
        if (selectedApp) {
          this.flattenData([selectedApp]);
        } else {
          this.dataSource = [];
        }
      });
  }

  ngOnDestroy() {
    if (this.selectedAppSubscription) {
      this.selectedAppSubscription.unsubscribe();
    }
  }
  dropdownOptions = ['Scoped', 'Preview', 'None'];
  selectedOption = this.dropdownOptions[0];

  onOptionSelected(option: string) {}
  getTagOptions(tag: string): string[] {
    return ['Scoped', 'Preview', 'None'];
  }

  onTagSelected(option: string, element: FlattenedVersion) {}

  flattenData(apps: App[]) {
    this.dataSource = apps.flatMap((app) =>
      app.Versions.map((version) => ({
        Version: version.Name,
        Tag: version.Tag,
        Name: app.Name,
        ID: app.ID,
        Modules: version.Modules,
        ParentApp: app,
        isLoading: false, // Initialize isLoading here
      }))
    );
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
  stopPropagation(event: Event) {
    event.stopPropagation();
  }
  handleMenuSelection(action: string, version: Version) {
    switch (action) {
      case 'publish':
        // Handle publish action
        break;
      case 'unpublish':
        // Handle unpublish action
        break;
      default:
        console.warn(`Unknown action: ${action}`);
    }
  }

  flattenedVersionToDto(flattenedVersion: FlattenedVersion): Version {
    type tag = 'scoped' | 'preview' | 'none';
    return {
      ID: `${
        flattenedVersion.Tag === ''
          ? flattenedVersion.Version
          : flattenedVersion.Version + '-' + flattenedVersion.Tag
      }`,
      Modules: flattenedVersion.Modules,
      Name: flattenedVersion.Version,
      Tag: flattenedVersion.Tag as tag,
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
        const versionDtoFiltered: Version = {
          ...versionDto,
          Modules: versionDto.Modules.filter((mod) => !mod.IsPublished),
        };

        this.appService.publishVersion(versionDtoFiltered).subscribe({
          next: (response) => {
            if (response.success) {
              this.snackBar.open('Version published successfully', 'Close', {
                duration: 3000,
              });
              // Update local state
              flattenedVersion.Modules.forEach((module) => {
                module.IsPublished = true;
              });
              this.updateDataSource();
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
            this.updateDataSource();
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
        const versionDtoFiltered: Version = {
          ...versionDto,
          Modules: versionDto.Modules.filter((mod) => mod.IsPublished),
        };
        this.appService.unPublishVersion(versionDtoFiltered).subscribe({
          next: (response) => {
            if (response.success) {
              this.snackBar.open('Version unpublished successfully', 'Close', {
                duration: 3000,
              });
              // Update local state
              flattenedVersion.Modules.forEach((module) => {
                module.IsPublished = false;
              });
              this.updateDataSource();
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
            this.updateDataSource();
          },
        });
      }
    });
  }

  private updateDataSource() {
    // Create a new reference to trigger change detection
    this.dataSource = [...this.dataSource];
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
    const isPublished = this.getPublicationStatus(version);

    if (isPublished == 'published') {
      return 'check_circle';
    }
    if (isPublished == 'not-published') {
      return 'cancel';
    } else {
      return 'remove';
    }
  }
  getPublicationColor(version: FlattenedVersion): string {
    const isPublished = this.getPublicationStatus(version);

    if (isPublished == 'published') {
      return '#1b701e';
    }
    if (isPublished == 'not-published') {
      return '#f53c37';
    } else {
      return 'orange';
    }
  }
}
