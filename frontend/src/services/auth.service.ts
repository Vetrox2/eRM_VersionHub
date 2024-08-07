import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import Keycloak from 'keycloak-js';
import { BehaviorSubject, Observable } from 'rxjs';
import { KeycloakService } from 'keycloak-angular';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  // private tokenSubject = new BehaviorSubject<string | undefined>(undefined);

  constructor(private readonly keycloak: KeycloakService) {}

  isAuthenticated(): boolean {
    return this.keycloak.isLoggedIn() ?? false;
  }
  isLoggedIn(): boolean {
    return this.keycloak.isLoggedIn();
  }
  logout(): void {
    this.keycloak.logout();
  }
  getUserProfile(): any {
    return this.keycloak.loadUserProfile();
  }

  login(): void {
    this.keycloak.login();
  }

  // getToken(): string | undefined {
  //   return this.tokenSubject.value;
  // }

  // getToken$(): Observable<string | undefined> {
  //   return this.tokenSubject.asObservable();
  // }
}
