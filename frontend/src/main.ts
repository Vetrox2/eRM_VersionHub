import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';
import { APP_INITIALIZER } from '@angular/core';
import { AuthService } from './services/auth.service';

function initializeKeycloak(authService: AuthService) {
  return () => authService.initializeKeycloak();
}

const extendedConfig = {
  ...appConfig,
  providers: [
    ...appConfig.providers,
    {
      provide: APP_INITIALIZER,
      useFactory: initializeKeycloak,
      multi: true,
      deps: [AuthService],
    },
  ],
};

bootstrapApplication(AppComponent, extendedConfig).catch((err) =>
  console.error(err)
);
