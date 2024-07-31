import { Component } from '@angular/core';
import { AdminDashboardComponent } from '../../features/dashboard/admin-dashboard/admin-dashboard.component';

@Component({
  selector: 'app-admin-page',
  standalone: true,
  imports: [AdminDashboardComponent],
  templateUrl: './admin-page.component.html',
  styleUrl: './admin-page.component.scss',
})
export class AdminPageComponent {}
