import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatButtonModule],
  template: `
    <div class="error-container">
      <mat-card class="error-card">
        <mat-card-header>
          <mat-card-title>Unauthorized Access</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <p>You don't have permission to access this page.</p>
        </mat-card-content>
        <mat-card-actions>
          <button mat-raised-button color="primary" routerLink="/">
            Go to Home
          </button>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: [`
    .error-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 50vh;
    }
    .error-card {
      max-width: 400px;
      text-align: center;
    }
  `]
})
export class UnauthorizedComponent {}