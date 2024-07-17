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

interface FlattenedVersion {
  Version: string;
  Description: string;
  Name: string;
  ID: string;
  Modules: App['Versions'][0]['Modules'];
  ParentApp: App;
  Tag: string;
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
  ],
})
export class ProjectVersionTableComponent implements OnInit, OnDestroy {
  dataSource: FlattenedVersion[] = [];
  columnsToDisplay = ['Actions', 'Published', 'Version', 'Tag', 'Name', 'ID'];
  expandedElement: FlattenedVersion | null = null;
  private selectedAppSubscription: Subscription | undefined;

  menuItems: MenuItem[] = [
    { icon: 'publish', label: 'Publish', action: 'publish' },
    { icon: 'cancel', label: 'Unpublish', action: 'unpublish' },
  ];

  constructor(private appService: AppService) {}

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
        Description: app.Description,
        Name: app.Name,
        ID: app.ID,
        Modules: version.Modules,
        ParentApp: app,
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
  getPublicationIcon(version: FlattenedVersion): string {
    const isPublished = this.getPublicationStatus(version) === 'published';

    return isPublished ? 'check' : 'close';
  }
}
