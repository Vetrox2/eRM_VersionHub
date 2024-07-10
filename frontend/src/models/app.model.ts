import { Version } from './version.model';

export interface App {
  ID: string;
  Name: string;
  Versions: Version[];
  Description: string;
  IsFavourite: boolean;
}
