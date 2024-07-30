import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import Keycloak from 'keycloak-js';
import { initializeKeycloak } from '../keycloak.config';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private keycloakInstance: Keycloak | null = null;
  private isInitialized = false;
  private tokenSubject = new BehaviorSubject<string | undefined>(undefined);

  constructor(
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    if (isPlatformBrowser(this.platformId)) {
      this.keycloakInstance = initializeKeycloak();
    }
  }

  isAuthenticated(): boolean {
    return this.keycloakInstance?.authenticated ?? false;
  }

  login(): void {
    this.keycloakInstance?.login();
  }

  logout(): void {
    this.keycloakInstance?.logout({
      redirectUri: window.location.origin + '/login',
    });
  }

  getToken(): string | undefined {
    return this.tokenSubject.value;
  }

  getToken$(): Observable<string | undefined> {
    return this.tokenSubject.asObservable();
  }

  initializeKeycloak(): Promise<boolean> {
    if (!isPlatformBrowser(this.platformId)) {
      return Promise.resolve(false);
    }

    return new Promise((resolve, reject) => {
      this.keycloakInstance
        ?.init({ onLoad: 'check-sso' })
        .then((auth) => {
          this.isInitialized = true;
          this.tokenSubject.next(this.keycloakInstance?.token);

          if (!auth) {
            this.router.navigate(['/login']);
          } else {
            this.router.navigate(['/dashboard']);
          }
          resolve(auth);
        })
        .catch(reject);
    });
  }
}
