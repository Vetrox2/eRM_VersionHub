import { TestBed } from '@angular/core/testing';
import { VersionUtilsService } from './version-utils.service';
import { FlattenedVersion } from '../models/flattened-version.model';
import { App } from '../models/app.model';
import { mockApps } from '../testing/mock-data';

describe('VersionUtilsService', () => {
  let service: VersionUtilsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(VersionUtilsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getPublicationStatus', () => {
    it('should return "published" when all modules are published', () => {
      const version: FlattenedVersion = {
        Modules: [
          { Name: 'Module1', IsPublished: true, IsOptional: false },
          { Name: 'Module2', IsPublished: true, IsOptional: false },
        ],
      } as FlattenedVersion;

      expect(service.getPublicationStatus(version)).toBe('published');
    });

    it('should return "semi-published" when some modules are published', () => {
      const version: FlattenedVersion = {
        Modules: [
          { Name: 'Module1', IsPublished: true, IsOptional: false },
          { Name: 'Module2', IsPublished: false, IsOptional: false },
        ],
      } as FlattenedVersion;

      expect(service.getPublicationStatus(version)).toBe('semi-published');
    });

    it('should return "not-published" when no modules are published', () => {
      const version: FlattenedVersion = {
        Modules: [
          { Name: 'Module1', IsPublished: false, IsOptional: false },
          { Name: 'Module2', IsPublished: false, IsOptional: false },
        ],
      } as FlattenedVersion;

      expect(service.getPublicationStatus(version)).toBe('not-published');
    });
  });

  describe('getPublicationStatusText', () => {
    it('should return "Published" when all modules are published', () => {
      const version: FlattenedVersion = {
        Modules: [
          { Name: 'Module1', IsPublished: true, IsOptional: false },
          { Name: 'Module2', IsPublished: true, IsOptional: false },
        ],
      } as FlattenedVersion;

      expect(service.getPublicationStatusText(version)).toBe('Published');
    });

    it('should return "Partially Published" when some modules are published', () => {
      const version: FlattenedVersion = {
        Modules: [
          { Name: 'Module1', IsPublished: true, IsOptional: false },
          { Name: 'Module2', IsPublished: false, IsOptional: false },
        ],
      } as FlattenedVersion;

      expect(service.getPublicationStatusText(version)).toBe(
        'Partially Published'
      );
    });

    it('should return empty string when no modules are published', () => {
      const version: FlattenedVersion = {
        Modules: [
          { Name: 'Module1', IsPublished: false, IsOptional: false },
          { Name: 'Module2', IsPublished: false, IsOptional: false },
        ],
      } as FlattenedVersion;

      expect(service.getPublicationStatusText(version)).toBe('');
    });
  });

  describe('getPublicationStatusValue', () => {
    it('should return correct values for different statuses', () => {
      expect(service.getPublicationStatusValue('published')).toBe(2);
      expect(service.getPublicationStatusValue('semi-published')).toBe(1);
      expect(service.getPublicationStatusValue('not-published')).toBe(0);
    });
  });

  describe('flattenedVersionToDto', () => {
    it('should convert FlattenedVersion to Version DTO', () => {
      const flattenedVersion: FlattenedVersion = {
        orignalID: '1',
        Version: '1.0.0',
        Modules: [],
        Tag: 'preview',
        ID: '1',
        isLoading: false,
        Name: 'das',
        ParentApp: mockApps[0],
      } as FlattenedVersion;

      const result = service.flattenedVersionToDto(flattenedVersion);

      expect(result).toEqual({
        ID: '1',
        Number: '1.0.0',
        Modules: [],
        Name: '1.0.0',
        PublishedTag: 'preview',
      });
    });
  });

  describe('flattenData', () => {
    it('should flatten app data', () => {
      const apps: App[] = [
        {
          ID: '1',
          Name: 'App1',
          Versions: [
            {
              ID: 'v1',
              Number: '1.0.0',
              Modules: [],
              PublishedTag: 'preview',
              Name: 'Version1',
            },
          ],
          IsFavourite: false,
        },
      ];

      const result = service.flattenData(apps);

      expect(result.length).toBe(1);
      expect(result[0]).toEqual({
        Version: '1.0.0',
        Tag: 'preview',
        Name: 'App1',
        ID: '1',
        Modules: [],
        ParentApp: apps[0],
        isLoading: false,
        orignalID: 'v1',
      });
    });
  });

  describe('getPublicationIcon', () => {
    it('should return correct icons for different statuses', () => {
      const publishedVersion: FlattenedVersion = {
        Modules: [{ Name: 'Module1', IsPublished: true, IsOptional: false }],
      } as FlattenedVersion;
      const semiPublishedVersion: FlattenedVersion = {
        Modules: [
          { Name: 'Module1', IsPublished: true, IsOptional: false },
          { Name: 'Module2', IsPublished: false, IsOptional: false },
        ],
      } as FlattenedVersion;
      const notPublishedVersion: FlattenedVersion = {
        Modules: [{ Name: 'Module1', IsPublished: false, IsOptional: false }],
      } as FlattenedVersion;

      expect(service.getPublicationIcon(publishedVersion)).toBe('check_circle');
      expect(service.getPublicationIcon(semiPublishedVersion)).toBe('remove');
      expect(service.getPublicationIcon(notPublishedVersion)).toBe('cancel');
    });
  });

  describe('getPublicationColor', () => {
    it('should return correct colors for different statuses', () => {
      const publishedVersion: FlattenedVersion = {
        Modules: [{ Name: 'Module1', IsPublished: true, IsOptional: false }],
      } as FlattenedVersion;
      const semiPublishedVersion: FlattenedVersion = {
        Modules: [
          { Name: 'Module1', IsPublished: true, IsOptional: false },
          { Name: 'Module2', IsPublished: false, IsOptional: false },
        ],
      } as FlattenedVersion;
      const notPublishedVersion: FlattenedVersion = {
        Modules: [{ Name: 'Module1', IsPublished: false, IsOptional: false }],
      } as FlattenedVersion;

      expect(service.getPublicationColor(publishedVersion)).toBe('#1b701e');
      expect(service.getPublicationColor(semiPublishedVersion)).toBe('orange');
      expect(service.getPublicationColor(notPublishedVersion)).toBe('#f53c37');
    });
  });
});
