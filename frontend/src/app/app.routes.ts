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
      {
        path: 'admin',
        loadComponent: () =>
          import(
            '../features/dashboard/admin-dashboard/admin-dashboard/admin-dashboard.component'
          ).then((m) => m.AdminDashboardComponent),
      },
    ],
  },
  { path: '**', redirectTo: '/login' },
];
