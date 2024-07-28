import { App } from '../models/app.model';

export const mockApps: App[] = [
  {
    ID: 'id1',
    IsFavourite: true,
    Name: 'app1',
    Versions: [
      {
        ID: '1.2.3.4-preview',
        Modules: [
          { IsOptional: false, IsPublished: false, Name: 'Module1' },
          { IsOptional: false, IsPublished: false, Name: 'Module2' },
          { IsOptional: false, IsPublished: false, Name: 'Module3' },
        ],
        Name: 'app1',
        Number: '1.2.3.4',
        PublishedTag: '',
      },
    ],
  },
  {
    ID: 'id2',
    IsFavourite: false,
    Name: 'app2',
    Versions: [
      {
        ID: '2.0.0',
        Modules: [{ IsOptional: false, IsPublished: true, Name: 'Module1' }],
        Name: 'app2',
        Number: '2.0.0',
        PublishedTag: '',
      },
    ],
  },
];
