import { Injectable } from '@angular/core';
import { Tag, Version } from '../models/version.model';
import { App } from '../models/app.model';
import { FlattenedVersion } from '../models/flattened-version.model';

@Injectable({
  providedIn: 'root',
})
export class VersionUtilsService {
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
  getPublicationStatusText(version: FlattenedVersion) {
    const publishedCount = version.Modules.filter((m) => m.IsPublished).length;
    if (publishedCount === version.Modules.length) {
      return 'Published';
    } else if (publishedCount > 0) {
      return 'Partially Published';
    } else {
      return '';
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

  flattenedVersionToDto(flattenedVersion: FlattenedVersion): Version {
    return {
      ID: flattenedVersion.orignalID,
      Number: flattenedVersion.Version,
      Modules: flattenedVersion.Modules,
      Name: flattenedVersion.Version,
      PublishedTag:
        flattenedVersion.Tag === 'none' ? '' : (flattenedVersion.Tag as Tag),
    };
  }

  flattenData(apps: App[]): FlattenedVersion[] {
    return apps.flatMap((app) =>
      app.Versions.map((version) => ({
        Version: version.Number,
        Tag: version.PublishedTag,
        Name: app.Name,
        ID: app.ID,
        Modules: version.Modules,
        ParentApp: app,
        isLoading: false,
        orignalID: version.ID,
      }))
    );
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
