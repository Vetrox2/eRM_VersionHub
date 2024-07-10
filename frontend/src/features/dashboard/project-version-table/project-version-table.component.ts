import { Component, OnInit, OnDestroy } from '@angular/core';
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

interface FlattenedVersion {
  Version: string;
  Description: string;
  Name: string;
  ID: string;
  Modules: App['Versions'][0]['Modules'];
  ParentApp: App;
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
  ],
})
export class ProjectVersionTableComponent implements OnInit, OnDestroy {
  dataSource: FlattenedVersion[] = [];
  columnsToDisplay = ['Version', 'Description', 'Name', 'ID', 'Published'];
  expandedElement: FlattenedVersion | null = null;
  private selectedAppSubscription: Subscription | undefined;

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

  flattenData(apps: App[]) {
    this.dataSource = apps.flatMap((app) =>
      app.Versions.map((version) => ({
        Version: version.ID,
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
}
