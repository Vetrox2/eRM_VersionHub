import {
  HttpClient,
  HttpHeaders,
  HttpErrorResponse,
} from '@angular/common/http';
import { environment } from '../environments/environment';
import { catchError, Observable, throwError } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private apiBaseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  get<T>(endpoint: string): Observable<T> {
    return this.http
      .get<T>(`${this.apiBaseUrl}/${endpoint}`)
      .pipe(catchError((error) => this.handleError<T>(error)));
  }

  post<T>(endpoint: string, body: any): Observable<T> {
    return this.http
      .post<T>(`${this.apiBaseUrl}/${endpoint}`, body)
      .pipe(catchError((error) => this.handleError<T>(error)));
  }

  delete<T>(endpoint: string, body?: any): Observable<T> {
    const options = body
      ? {
          body,
          headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
        }
      : {};
    return this.http
      .delete<T>(`${this.apiBaseUrl}/${endpoint}`, options)
      .pipe(catchError((error) => this.handleError<T>(error)));
  }

  private handleError<T>(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred';
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    console.error(errorMessage);
    // Instead of returning a default ApiResponse, we're throwing an error
    return throwError(() => new Error(errorMessage));
  }
}
