import { App } from './app.model';

export interface FlattenedVersion {
  Version: string;
  Name: string;
  ID: string;
  Modules: App['Versions'][0]['Modules'];
  ParentApp: App;
  Tag: string;
  isLoading: boolean;
  orignalID: string;
}
