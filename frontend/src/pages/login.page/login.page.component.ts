import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login.page',
  standalone: true,
  imports: [],
  template: `<div>
    <h2>Welcome to Our App</h2>
    <p>Please log in to continue</p>
    <button (click)="login()">Login</button>
  </div>`,
  // templateUrl: './login.page.component.html',
  styleUrl: './login.page.component.scss',
})
export class LoginPageComponent {
  constructor(private authService: AuthService) {}

  login(): void {
    this.authService.login();
  }
}
