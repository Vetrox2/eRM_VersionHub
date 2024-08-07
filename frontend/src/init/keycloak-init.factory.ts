import { KeycloakService } from 'keycloak-angular';

declare var require: any;
const Keycloak = typeof window !== 'undefined' ? import('keycloak-js') : null;

export function initializeKeycloak(keycloak: KeycloakService) {
  return () =>
    new Promise<boolean>((resolve) => {
      if (typeof window !== 'undefined') {
        keycloak
          .init({
            config: {
              url: 'http://localhost:8080',
              realm: 'eRM-realm',
              clientId: 'angular-client',
            },
            enableBearerInterceptor: true,
            bearerPrefix: 'Bearer',
            bearerExcludedUrls: ['/assets'],
            initOptions: {
              onLoad: 'check-sso',
              silentCheckSsoRedirectUri:
                window.location.origin + '/assets/silent-check-sso.html',
            },
          })
          .then((authenticated) => {
            resolve(authenticated);
          })
          .catch(() => {
            resolve(false);
          });
      } else {
        resolve(false);
      }
    });
}
