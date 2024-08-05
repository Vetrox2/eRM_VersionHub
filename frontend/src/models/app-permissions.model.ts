export interface AppPermission {
  User: string;
  AppsPermission: {
    [key: string]: boolean;
  };
}
