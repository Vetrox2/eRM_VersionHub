import { inject } from '@angular/core';
import { CanMatchFn, Router, UrlTree } from '@angular/router';
import { KeycloakService } from 'keycloak-angular';
import { isPlatformBrowser } from '@angular/common';
import { PLATFORM_ID } from '@angular/core';

export const authGuard: CanMatchFn = async (
  route,
  segments
): Promise<boolean | UrlTree> => {
  const router = inject(Router);
  const keycloakService = inject(KeycloakService);
  const platformId = inject(PLATFORM_ID);

  if (isPlatformBrowser(platformId)) {
    // Client-side code
    // Wait for Keycloak initialization

    const authenticated: boolean = await keycloakService.isLoggedIn();

    if (!authenticated) {
      // If not authenticated and trying to access login page, allow it
      if (segments[0]?.path === 'login') {
        return true;
      }
      // Otherwise, redirect to login page
      return router.createUrlTree(['/login']);
    }

    // If authenticated and trying to access login page, redirect to dashboard
    if (segments[0]?.path === 'login') {
      return router.createUrlTree(['/dashboard']);
    }

    // Get the user Keycloak roles and the required roles from the route
    const roles: string[] = keycloakService.getUserRoles(true);
    const requiredRoles = route.data?.['roles'];

    // Allow the user to proceed if no additional roles are required to access the route
    if (!Array.isArray(requiredRoles) || requiredRoles.length === 0) {
      return true;
    }

    // Allow the user to proceed if ALL of the required roles are present
    const authorized = requiredRoles.every((role) => roles.includes(role));

    if (authorized) {
      return true;
    }

    // Redirect to an access denied page or handle unauthorized access as needed
    return router.createUrlTree(['/access-denied']);
  } else {
    // Server-side code
    // Always allow navigation during SSR to avoid blocking routes
    return true;
  }
};
