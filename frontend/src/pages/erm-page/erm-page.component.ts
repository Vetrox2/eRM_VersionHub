import { Component } from '@angular/core';
import { ErmDashboardPageComponent } from '../../features/dashboard/erm-dashboard/erm-dashboard.component';

@Component({
  selector: 'app-erm-page',
  standalone: true,
  imports: [ErmDashboardPageComponent],
  templateUrl: './erm-page.component.html',
  styleUrl: './erm-page.component.scss',
})
export class ErmPageComponent {}
