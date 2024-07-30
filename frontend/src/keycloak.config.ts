import Keycloak from 'keycloak-js';

export const keycloakConfig: Keycloak.KeycloakConfig = {
  url: 'http://localhost:8080',
  realm: 'eRM-realm',
  clientId: 'angular-client',
};

export function initializeKeycloak(): Keycloak {
  return new Keycloak(keycloakConfig);
}
