import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../environments/environment';
import { catchError, map, Observable, of } from 'rxjs';
import { App } from '../models/app.model';
import { ApiResponse } from '../models/api-response.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private apiBaseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  get<T>(endpoint: string): Observable<T> {
    return this.http.get<T>(`${this.apiBaseUrl}/${endpoint}`);
  }

  post<T>(endpoint: string, body: any): Observable<T | ApiResponse<any>> {
    return this.http
      .post<T>(`${this.apiBaseUrl}/${endpoint}`, body)
      .pipe(catchError(this.handleError));
  }

  delete<T>(endpoint: string, body?: any): Observable<T | ApiResponse<any>> {
    const options = body
      ? {
          body,
          headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
        }
      : {};
    return this.http
      .delete<T>(`${this.apiBaseUrl}/${endpoint}`, options)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any): Observable<ApiResponse<any>> {
    let errorMessage = 'An unknown error occurred';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    return of({ Success: false, Data: null, Errors: [errorMessage] });
  }
}
