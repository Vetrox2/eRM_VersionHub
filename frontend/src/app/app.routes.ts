import { Routes } from '@angular/router';
import { LoginPageComponent } from '../pages/login-page/login-page.component';
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
          import('../pages/erm-page/erm-page.component').then(
            (m) => m.ErmPageComponent
          ),
      },
      {
        path: 'admin',
        loadComponent: () =>
          import('../pages/admin-page/admin-page.component').then(
            (m) => m.AdminPageComponent
          ),
      },
    ],
  },
  { path: '**', redirectTo: '/login' },
];
