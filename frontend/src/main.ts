import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { AppService } from './services/app.service';

bootstrapApplication(AppComponent, appConfig).catch((err) =>
  console.error(err)
);
