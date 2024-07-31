import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {
  HttpClientModule,
  provideHttpClient,
  withInterceptors,
} from '@angular/common/http';
import { AppService } from '../services/app.service';
import { tokenInterceptor } from '../token.interceptor';
import { AuthService } from '../services/auth.service';
import { KeycloakService, KeycloakAngularModule } from 'keycloak-angular';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([tokenInterceptor])),
    provideClientHydration(),
    provideAnimationsAsync(),
    importProvidersFrom(HttpClientModule, KeycloakAngularModule),
    KeycloakService,
    AuthService,
    AppService,
  ],
};
