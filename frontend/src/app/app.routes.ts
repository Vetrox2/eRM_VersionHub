import { Routes } from '@angular/router';
import { LoginPageComponent } from '../pages/login.page/login.page.component';
import { AuthGuard } from '../auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginPageComponent },
  {
    path: '',
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import(
            '../pages/erm-dashboard.page/erm-dashboard.page.component'
          ).then((m) => m.ErmDashboardPageComponent),
      },
    ],
  },
  { path: '**', redirectTo: '/login' },
];
