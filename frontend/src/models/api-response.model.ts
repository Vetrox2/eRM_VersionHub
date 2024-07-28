export interface ApiResponse<T> {
  Success: boolean;
  Data: T;
  Errors: string[];
}
