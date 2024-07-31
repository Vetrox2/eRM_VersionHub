import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Version } from '../models/version.model';
import { ApiResponse } from '../models/api-response.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PublicationService {
  constructor(private apiService: ApiService) {}

  publishVersion(version: Version): Observable<ApiResponse<any>> {
    return this.apiService.post('Publication', version);
  }

  unPublishVersion(version: Version): Observable<ApiResponse<any>> {
    return this.apiService.delete('Publication', version);
  }
}
