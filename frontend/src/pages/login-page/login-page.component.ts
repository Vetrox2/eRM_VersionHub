import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [MatButtonModule, MatCardModule, MatIconModule],
  template: `
    <div class="login-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Welcome to Our App</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <p>Please log in to continue</p>
        </mat-card-content>
        <mat-card-actions>
          <button mat-raised-button color="primary" (click)="login()">
            <mat-icon>login</mat-icon>
            Login
          </button>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: [
    `
      .login-container {
        display: flex;
        justify-content: center;
        align-items: center;
        height: 100vh;
        background-color: #f5f5f5;
      }
      mat-card {
        max-width: 400px;
        width: 100%;
        padding: 20px;
      }
      mat-card-title {
        font-size: 24px;
        margin-bottom: 16px;
      }
      mat-card-content {
        margin-bottom: 16px;
      }
      button {
        width: 100%;
      }
    `,
  ],
})
export class LoginPageComponent {
  constructor(private authService: AuthService) {}

  login(): void {
    this.authService.login();
  }
}
